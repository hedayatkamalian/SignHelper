using IdGen;
using Microsoft.Extensions.Options;
using MustIt.Kernel.Services;
using MustIt.Kernel.Services.Interfaces;
using MustKernel.Helpers;
using SignHelperApp.Commands.SignRequests;
using SignHelperApp.Commands.Templates;
using SignHelperApp.DTO;
using SignHelperApp.Entities;
using SignHelperApp.Exceptions;
using SignHelperApp.Options;
using SignHelperApp.Repositories.Interfaces;
using SignHelperApp.Services.Interfaces;
using SignHelperApp.Types;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.Json;


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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationOptions _applicationOptions;
        private readonly ApplicationErrors _applicationErrors;

        public SignHelperService(ITemplatesRepository templateRepository,
            ISignRequestsRepository signRequestRepository,
            IEmailService emailService,
            IdGenerator idGenerator,
            IDownloaderService downloaderService,
            ISmsService smsService,
            IHttpClientFactory httpClientFactory,
            IOptions<ApplicationOptions> applicationOptions,
            IOptions<ApplicationErrors> applicationErrors)
        {
            _templateRepository = templateRepository;
            _signRequestRepository = signRequestRepository;
            _emailService = emailService;
            _idGenerator = idGenerator;
            _downloaderService = downloaderService;
            _smsService = smsService;
            _httpClientFactory = httpClientFactory;
            _applicationOptions = applicationOptions.Value;
            _applicationErrors = applicationErrors.Value;
        }

        public async Task<ServiceResult<string>> TemplateAdd(TemplateAddCommand command)
        {
            try
            {
                var template = new Template
                {
                    Id = _idGenerator.CreateId(),
                    Name = command.Name,
                    StampName = command.ImageName,
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
            catch (Exception ex)
            {
                return new ServiceResult<string>(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        public async Task<ServiceResult<string>> TemplateDelete(long id)
        {
            try
            {
                var template = await _templateRepository.GetAsync(id);

                if (template is null)
                    return new ServiceResult<string>(HttpStatusCode.NotFound);

                template.Deleted = true;

                var deleteResult = await _templateRepository.SaveChangesAsync();

                return ResponseBaseOnResult<string>(deleteResult, HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        public async Task<ServiceResult<IList<Template>>> TemplateGetAll()
        {
            try
            {
                return new ServiceResult<IList<Template>>(await _templateRepository.GetAll(), HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IList<Template>>(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private async Task<SignRequest> SignRequestGetAndCheck(long id)
        {
            var signRequest = await _signRequestRepository.GetAsync(p => p.Id == id);

            if (signRequest is null)
            {
                throw new SignRequestException(HttpStatusCode.NotFound, _applicationErrors.SignRequestNotFound);
            }
            else if (signRequest.Done)
            {
                throw new SignRequestException(HttpStatusCode.UnprocessableEntity, _applicationErrors.DocumentIsSignedBefore);
            }
            else if (signRequest.ExpireIn < DateTimeOffset.UtcNow)
            {
                throw new SignRequestException(HttpStatusCode.UnprocessableEntity, _applicationErrors.SignRequestIsExpired);
            }

            return signRequest;
        }

        public async Task<ServiceResult<SignRequestDto>> SignRequestGet(long id)
        {
            try
            {
                var signRequest = await SignRequestGetAndCheck(id);

                var dto = new SignRequestDto
                {
                    Id = signRequest.Id,
                    ConfirmCode = signRequest.ConfirmCode,
                    Description = signRequest.Description,
                    Email = signRequest.RecipientEmail,
                    ExpireIn = signRequest.ExpireIn.ToString(),
                };

                return new ServiceResult<SignRequestDto>(dto, HttpStatusCode.OK);
            }
            catch (SignRequestException ex)
            {
                return new ServiceResult<SignRequestDto>(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return new ServiceResult<SignRequestDto>(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<ServiceResult<int>> SignRequestSendConfirmCode(long id)
        {
            try
            {
                var signRequest = await SignRequestGetAndCheck(id);

                var confirmCode = GenerateConfirmCode();

                if (signRequest.ConfirmCodeExpireIn == null || signRequest.ConfirmCodeExpireIn < DateTimeOffset.UtcNow)
                {
                    signRequest.ConfirmCodeExpireIn = DateTimeOffset.UtcNow.AddSeconds(_applicationOptions.ConfirmCodeOptions.ExpireInSeconds);
                    signRequest.ConfirmCode = confirmCode;
                }
                else
                {
                    throw new SignRequestException(HttpStatusCode.UnprocessableEntity, _applicationErrors.YouShouldWaitToRequestNewCode);
                }

                await _signRequestRepository.SaveChangesAsync();

                var paraDic = new Dictionary<string, string>();
                paraDic.Add("code", confirmCode);

#if (!DEBUG)

                await _smsService.SendTemplateMessage(signRequest.SignerPhoneNumber,
                    _applicationOptions.ConfirmCodeTemplateName, paraDic);
#endif

                return new ServiceResult<int>(_applicationOptions.ConfirmCodeOptions.ExpireInSeconds, HttpStatusCode.OK);
            }
            catch (SignRequestException ex)
            {
                return new ServiceResult<int>(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return new ServiceResult<int>(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<ServiceResult<SignRequestDto>> SignRequestGetWithCode(long id, string code)
        {
            try
            {
                var signRequest = await SignRequestGetAndCheck(id);

                var dto = new SignRequestDto
                {
                    Id = signRequest.Id,
                    ConfirmCode = signRequest.ConfirmCode,
                    Description = signRequest.Description,
                    Email = signRequest.RecipientEmail,
                    ExpireIn = signRequest.ExpireIn.ToString(),
                };

                return new ServiceResult<SignRequestDto>(dto, HttpStatusCode.OK);
            }
            catch (SignRequestException ex)
            {
                return new ServiceResult<SignRequestDto>(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return new ServiceResult<SignRequestDto>(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<ServiceResult<string>> SignRequestAdd(SignRequestAddCommand command)
        {
            try
            {
                var id = _idGenerator.CreateId();

                var dirPath = PathHelper.CreateTempWorkingDirectory(_applicationOptions.Folders.Temp, id.ToString(), true);

                var fileName = $"{_applicationOptions.PrefixOptions.Original}{id}.pdf";
                var filePath = await _downloaderService.Download(command.FileURL, dirPath, fileName);


                var templateIdExist = await _templateRepository.AnyAsync(p => p.Id == command.SignTemplateId && !p.Deleted);

                if (!templateIdExist)
                {
                    return new ServiceResult<string>(HttpStatusCode.UnprocessableEntity, _applicationErrors.TemplateIdDoesNotExist);
                }

                var signRequest = new SignRequest
                {
                    Id = id,
                    RecipientEmail = command.RecipientEmail,
                    SignerEmail = command.SingerEmail,
                    SignerPhoneNumber = command.SingerPhoneNumber,
                    Description = command.Description,
                    ExpireIn = DateTimeOffset.UtcNow.AddDays(_applicationOptions.ExpireInDays),
                    ConfirmCodeExpireIn = null,
                    ConfirmCode = GenerateConfirmCode(),
                    Done = false,
                    SignTemplateId = command.SignTemplateId,
                    StampTemplateId = command.StampTemplateId
                };

                await _signRequestRepository.AddAsync(signRequest);
                var saveResult = await _signRequestRepository.SaveChangesAsync();
                var singLink = @$"{System.Environment.NewLine} {_applicationOptions.ConfirmApiAddress}/{signRequest.Id}";


                var email = new Email
                {
                    SenderEmail = _applicationOptions.SendDraftDocument.SenderEmail,
                    SenderName = _applicationOptions.SendDraftDocument.SenderName,
                    To = new List<string>() { signRequest.SignerEmail },
                    Subject = _applicationOptions.SendDraftDocument.Subject,
                    Cc = _applicationOptions.SendDraftDocument.Cc,
                    Bcc = _applicationOptions.SendDraftDocument.Bcc,
#if (!DEBUG)
                    AttachmentsUrl = new string[] { @$"{_applicationOptions.ApplicationBaseUrl}{PathHelper.GetRelationPath(filePath, false)}" },
#endif
                    HeaderPictureUrl = _applicationOptions.SendDraftDocument.HeaderPictureUrl,
                    Body = _applicationOptions.SendDraftDocument.Body + singLink
                };

                await _emailService.SendEmail(email);

                var response = ResponseBaseOnResult<string>(saveResult, HttpStatusCode.Created);
                response.Id = id.ToString();
                return response;
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>(HttpStatusCode.InternalServerError, ex.Message);
            }


        }

        public async Task<ServiceResult<string>> SingRequestConfirm(long id, string confirmCode)
        {
            try
            {
                var signRequest = await SignRequestGetAndCheck(id);

                if (signRequest.ConfirmCode != confirmCode)
                {
                    throw new SignRequestException(HttpStatusCode.UnprocessableEntity, _applicationErrors.ConfirmCodeIsWrong);
                }

                return new ServiceResult<string>(HttpStatusCode.OK);

            }
            catch (SignRequestException ex)
            {
                return new ServiceResult<string>(ex.StatusCode, ex.Message);

            }
            catch (Exception ex)
            {
                return new ServiceResult<string>(HttpStatusCode.InternalServerError, ex.Message);
            }


        }

        public async Task<ServiceResult<string>> SingRequestSign(SignRequestSignCommand command)
        {
            try
            {
                var signRequest = await SignRequestGetAndCheck(command.Id);

                if (signRequest.ConfirmCode != command.ConfirmCode)
                {
                    throw new SignRequestException(HttpStatusCode.UnprocessableEntity, _applicationErrors.ConfirmCodeIsWrong);
                }
                else if (command.SignImageData is null)
                {
                    throw new SignRequestException(HttpStatusCode.UnprocessableEntity, _applicationErrors.SignImageError);
                }

                var signImageFile = SaveSignImage(command);

                var signedDocument = SignDocument
                    (GetOriginalDocument(command.Id),
                    GetSignedDocument(command.Id),
                    signImageFile,
                    signRequest.SignTemplate.Width,
                    signRequest.SignTemplate.Height,
                    signRequest.SignTemplate.SignPoints);

                if (signRequest.StampTemplateId is not null)
                {
                    var stampTemplate = await _templateRepository.GetAsync(signRequest.StampTemplateId.Value);

                    signedDocument = SignDocument
                    (GetSignedDocument(command.Id),
                    GetDocument(command.Id, _applicationOptions.PrefixOptions.SignedAndStamped),
                    GetSelectedStamp(stampTemplate.StampName),
                    stampTemplate.Width,
                    stampTemplate.Height,
                    stampTemplate.SignPoints);
                }


                var to = _applicationOptions.SendDraftDocument.To ?? new List<string>();
                to.Add(signRequest.RecipientEmail);

                var email = new Email
                {
                    SenderEmail = _applicationOptions.SendSignedDocument.SenderEmail,
                    SenderName = _applicationOptions.SendSignedDocument.SenderName,
                    To = to,
                    Subject = _applicationOptions.SendSignedDocument.Subject,
                    Cc = _applicationOptions.SendSignedDocument.Cc,
                    Bcc = _applicationOptions.SendSignedDocument.Bcc,
#if (!DEBUG)
                    AttachmentsUrl = new string[] { $"{_applicationOptions.ApplicationBaseUrl}/{signedDocument}" },
#endif
                    HeaderPictureUrl = _applicationOptions.SendSignedDocument.HeaderPictureUrl,
                    Body = _applicationOptions.SendSignedDocument.Body,
                };

                await _emailService.SendEmail(email);


                signRequest.Done = true;
                var saveResult = await _signRequestRepository.SaveChangesAsync();

                await SignCallback(signRequest.Id, signedDocument);

                return ResponseBaseOnResult<string>(saveResult, HttpStatusCode.NoContent);
            }
            catch (SignRequestException ex)
            {
                return new ServiceResult<string>(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>(HttpStatusCode.InternalServerError, ex.Message);
            }


        }

        private string SaveSignImage(SignRequestSignCommand command)
        {
            string fileName = $"{command.Id}_sign.png";

            string signImagePath = GetDocumentFolder(command.Id) + fileName;

            using (FileStream fileStream = new FileStream(signImagePath, FileMode.Create))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(Convert.FromBase64String(command.SignImageData));
                    binaryWriter.Close();
                }
            }

            return signImagePath;
        }

        private string SignDocument(string originalDocument, string signedDocument, string signImage, int width, int height, IList<SignPoint> signPoints)
        {
            var doc = new PdfDocument();
            doc.LoadFromFile(originalDocument);
            using (doc)
            {
                using (Image image = Image.FromFile(signImage))
                {
                    Size size = new Size(width, height);

                    foreach (var point in signPoints)
                    {
                        PdfImage pdfImage = PdfImage.FromImage(image);
                        PdfPageBase page = doc.Pages[point.Page - 1];
                        PointF position = new PointF(point.X, point.Y);
                        page.Canvas.DrawImage(pdfImage, position, size);
                    }

                    doc.SaveToFile(signedDocument);
                }
            }

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

        private string GetSelectedStamp(string? name)
        {
            var selectedSign = $"{_applicationOptions.Folders.Stamps}/{name ?? ""}.png";
            var defaultSign = $"{_applicationOptions.Folders.Stamps}/{_applicationOptions.DefaultStampImageName}.png";

            var selected = File.Exists(selectedSign) ? selectedSign : defaultSign;
            return selected;
        }

        private async Task SignCallback(long id, string documentAddress)
        {
            var client = _httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(_applicationOptions.SignCallbackUrl))
            {
                var address = new Uri(_applicationOptions.SignCallbackUrl);

                var dtoAsString = new StringContent(
                JsonSerializer.Serialize(new SignedDocumentDto
                {
                    DocumentAddress = documentAddress,
                    SignRequestId = id
                }),
                Encoding.UTF8,
                System.Net.Mime.MediaTypeNames.Application.Json);


                var response = await client.PostAsync(address, dtoAsString);
            }

        }

        private string GenerateConfirmCode()
        {
            var random = new Random(DateTime.Now.Millisecond);
            var min = (int)Math.Pow(10, _applicationOptions.ConfirmCodeOptions.Length - 1);
            var max = (int)Math.Pow(10, _applicationOptions.ConfirmCodeOptions.Length) - 1;

            return random.Next(min, max).ToString();

            //return fullCode.Substring(0, Math.Min(fullCode.Length, _applicationOptions.ConfirmCodeOptions.Length));
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
