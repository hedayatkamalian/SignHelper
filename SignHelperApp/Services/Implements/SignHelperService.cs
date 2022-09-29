using IdGen;
using Microsoft.Extensions.Options;
using MustIt.Kernel.Services;
using MustIt.Kernel.Services.Interfaces;
using MustKernel.Helpers;
using SignHelperApp.Commands.SignRequests;
using SignHelperApp.Commands.Templates;
using SignHelperApp.Entities;
using SignHelperApp.Options;
using SignHelperApp.Repositories.Interfaces;
using SignHelperApp.Services.Interfaces;
using SignHelperApp.Types;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Drawing;
using System.Net;

namespace SignHelperApp.Services.Implements
{
    public class SignHelperService : ISignHelperService
    {
        private readonly ITemplatesRepository _templateRepository;
        private readonly ISignRequestsRepository _signRequestRepository;
        private readonly IEmailService _emailService;
        private readonly IdGenerator _idGenerator;
        private readonly IDownloaderService _downloaderService;
        private readonly ISmsService _smsService;
        private readonly ApplicationOptions _applicationOptions;
        private readonly ApplicationErrors _applicationErrors;

        public SignHelperService(ITemplatesRepository templateRepository,
            ISignRequestsRepository signRequestRepository,
            IEmailService emailService,
            IdGenerator idGenerator,
            IDownloaderService downloaderService,
            ISmsService smsService,
            IOptions<ApplicationOptions> applicationOptions,
            IOptions<ApplicationErrors> applicationErrors)
        {
            _templateRepository = templateRepository;
            _signRequestRepository = signRequestRepository;
            _emailService = emailService;
            _idGenerator = idGenerator;
            _downloaderService = downloaderService;
            _smsService = smsService;
            _applicationOptions = applicationOptions.Value;
            _applicationErrors = applicationErrors.Value;
        }

        public async Task<ServiceResult<string>> TemplateAdd(TemplateAddCommand command)
        {
            var template = new Template
            {
                Id = _idGenerator.CreateId(),
                Name = command.Name,
                SignImageName = command.ImageName,
                Height = command.Height,
                Width = command.Width,
                SignPoints = command.SignPoints,
                Deleted = false
            };

            await _templateRepository.AddAsync(template);
            var affectedRow = await _templateRepository.SaveChangesAsync();

            var result = ResponseBaseOnResult<string>(affectedRow, HttpStatusCode.Created, template.Id.ToString());
            result.Id = template.Id.ToString();
            return result;
        }


        public async Task<ServiceResult<string>> TemplateDelete(long id)
        {
            var template = await _templateRepository.GetAsync(id);

            if (template is null)
                return new ServiceResult<string>(HttpStatusCode.NotFound);

            template.Deleted = true;

            var deleteResult = await _templateRepository.SaveChangesAsync();

            return ResponseBaseOnResult<string>(deleteResult, HttpStatusCode.NoContent);

        }


        public async Task<ServiceResult<IList<Template>>> TemplateGetAll()
        {
            return new ServiceResult<IList<Template>>(await _templateRepository.GetAll(), HttpStatusCode.OK);
        }



        private async Task<Template?> TemplateFindByName(string name)
        {
            return await _templateRepository.GetAsync(p => p.Name.ToLower() == name.ToLower());
        }




        public async Task<ServiceResult<string>> SignRequestAdd(SignRequestAddCommand command)
        {
            var id = _idGenerator.CreateId();

            var dirPath = PathHelper.CreateTempWorkingDirectory(_applicationOptions.Folders.Temp, id.ToString(), true);

            var fileName = $"{_applicationOptions.PrefixOptions.Original}{id}.pdf";
            var filePath = await _downloaderService.Download(command.FileURL, dirPath, fileName);


            var templateIdExist = await _templateRepository.AnyAsync(p => p.Id == command.TemplateId && !p.Deleted);

            if (!templateIdExist)
            {
                return new ServiceResult<string>(HttpStatusCode.UnprocessableEntity, _applicationErrors.TemplateIdDoesNotExist);
            }

            var signRequest = new SignRequest
            {
                Id = id,
                Email = command.Email,
                Description = command.Description,
                ConfirmCode = GenerateConfirmCode(),
                ExpireIn = DateTimeOffset.Now.AddMinutes(_applicationOptions.ConfirmCodeOptions.ExpireInMinutes),
                Done = false,
                TemplateId = command.TemplateId
            };

            await _signRequestRepository.AddAsync(signRequest);
            var saveResult = await _signRequestRepository.SaveChangesAsync();

            var response = ResponseBaseOnResult<string>(saveResult, HttpStatusCode.Created);
            response.Id = id.ToString();

            var paraDic = new Dictionary<string, string>();
            paraDic.Add("api", _applicationOptions.ConfirmApiAddress);
            paraDic.Add("id", signRequest.Id.ToString());
            paraDic.Add("code", signRequest.ConfirmCode);
            paraDic.Add("desc", signRequest.Description);

            await _smsService.SendTemplateMessage(_applicationOptions.NotifyOptions.PhoneNumber
                , _applicationOptions.NotifyOptions.TemplateName, paraDic);

            return response;

        }


        public async Task<ServiceResult<string>> SingRequestSign(long id, string confirmCode)
        {
            var signRequest = await _signRequestRepository.GetAsync(p => p.Id == id && p.ConfirmCode == confirmCode);

            if (signRequest is null)
            {
                return new ServiceResult<string>(HttpStatusCode.NotFound);
            }
            else if (signRequest.Done)
            {
                return new ServiceResult<string>(HttpStatusCode.UnprocessableEntity, _applicationErrors.DocumentIsSignedBefore);
            }
            else if (signRequest.ExpireIn < DateTimeOffset.Now)
            {
                return new ServiceResult<string>(HttpStatusCode.UnprocessableEntity, _applicationErrors.SignRequestIsExpired);
            }

            var signedDocument = SignDocument
                (GetOriginalDocument(id),
                GetSignedDocument(id),
                GetSelectedSign(signRequest.Template.SignImageName),
                signRequest.Template.Width,
                signRequest.Template.Height,
                signRequest.Template.SignPoints);

            var to = _applicationOptions.SignedDocumentEmailOptions.To ?? new List<string>();
            to.Add(signRequest.Email);

            var email = new Email
            {
                SenderEmail = _applicationOptions.SignedDocumentEmailOptions.SenderEmail,
                SenderName = _applicationOptions.SignedDocumentEmailOptions.SenderName,
                To = to,
                Subject = _applicationOptions.SignedDocumentEmailOptions.Subject,
                Cc = _applicationOptions.SignedDocumentEmailOptions.Cc,
                Bcc = _applicationOptions.SignedDocumentEmailOptions.Bcc,
                AttachmentsUrl = new string[] { $"{_applicationOptions.ApplicationBaseUrl}/{signedDocument}" },
                HeaderPictureUrl = _applicationOptions.SignedDocumentEmailOptions.HeaderPictureUrl,
                Body = _applicationOptions.SignedDocumentEmailOptions.Body,
            };

            await _emailService.SendEmail(email);


            signRequest.Done = true;
            var saveResult = await _signRequestRepository.SaveChangesAsync();

            return ResponseBaseOnResult<string>(saveResult, HttpStatusCode.NoContent);

        }

        private string SignDocument(string originalDocument, string signedDocument, string signImage, int width, int height, IList<SignPoint> signPoints)
        {
            var doc = new PdfDocument();
            doc.LoadFromFile(originalDocument);

            Image image = Image.FromFile(GetSelectedSign(signImage));
            Size size = new Size(width, height);

            foreach (var point in signPoints)
            {
                PdfImage pdfImage = PdfImage.FromImage(image);
                PdfPageBase page = doc.Pages[point.Page];
                PointF position = new PointF(point.X, point.Y);
                page.Canvas.DrawImage(pdfImage, position, size);
            }

            doc.SaveToFile(signedDocument);

            return signedDocument;
        }

        private string GetOriginalDocument(long id)
        {
            return GetDocument(id, _applicationOptions.PrefixOptions.Original);
        }
        private string GetSignedDocument(long id)
        {
            return GetDocument(id, _applicationOptions.PrefixOptions.Signed);
        }

        private string GetDocument(long id, string prefix)
        {
            return $"{GetDocumentFolder(id)}{prefix}{id}.pdf";
        }

        private string GetDocumentFolder(long id)
        {
            return $"{_applicationOptions.Folders.Temp}/{id}/";
        }

        private string GetSelectedSign(string? name)
        {
            var selectedSign = $"{_applicationOptions.Folders.Signs}/{name ?? ""}.png";
            var defaultSign = $"{_applicationOptions.Folders.Signs}/{_applicationOptions.DefaultSignImageName}.png";

            return File.Exists(selectedSign) ? selectedSign : defaultSign;
        }


        private string GenerateConfirmCode()
        {
            //var fullCode = _idGenerator.CreateId().ToString();
            var fullCode = Guid.NewGuid().ToString();

            return fullCode.Substring(0, Math.Min(fullCode.Length, _applicationOptions.ConfirmCodeOptions.Length));
        }


        private ServiceResult<T> ResponseBaseOnResult<T>(int affectedRows, HttpStatusCode successCode, T? result = default)
        {
            if (affectedRows > 0)
            {
                if (result is null)
                {
                    return new ServiceResult<T>(successCode);
                }
                else
                {
                    return new ServiceResult<T>(result, successCode);
                }
            }
            else
            {
                return new ServiceResult<T>(HttpStatusCode.InternalServerError, _applicationErrors.NoRowAffected);
            }
        }
    }
}
