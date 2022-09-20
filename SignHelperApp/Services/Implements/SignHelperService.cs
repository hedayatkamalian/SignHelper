using IdGen;
using Microsoft.Extensions.Options;
using MustIt.Kernel.Services;
using MustIt.Kernel.Services.Interfaces;
using MustKernel.Helpers;
using SignHelperApp.Command.SignRequest;
using SignHelperApp.Command.Template;
using SignHelperApp.Entities;
using SignHelperApp.Options;
using SignHelperApp.Repositories.Interfaces;
using SignHelperApp.Services.Interfaces;
using System.Net;

namespace SignHelperApp.Services.Implements
{
    public class SignHelperService : ISignHelperService
    {
        private readonly ITemplatesRepository _templateRepository;
        private readonly ISignRequestsRepository _signRequestRepository;
        private readonly IdGenerator _idGenerator;
        private readonly IDownloaderService _downloaderService;
        private readonly ApplicationOptions _applicationOptions;
        private readonly ApplicationErrors _applicationErrors;

        public SignHelperService(ITemplatesRepository templateRepository,
            ISignRequestsRepository signRequestRepository,
            IdGenerator idGenerator,
            IDownloaderService downloaderService,
            IOptions<ApplicationOptions> applicationOptions,
            IOptions<ApplicationErrors> applicationErrors)
        {
            _templateRepository = templateRepository;
            _signRequestRepository = signRequestRepository;
            _idGenerator = idGenerator;
            _downloaderService = downloaderService;
            _applicationOptions = applicationOptions.Value;
            _applicationErrors = applicationErrors.Value;
        }

        public async Task<ServiceResult<string>> TemplateAdd(TemplateAddCommand command)
        {
            var template = new Template
            {
                Id = _idGenerator.CreateId(),
                Name = command.Name,
                XPosition = command.XPosition,
                YPosition = command.YPosition,
                Deleted = false
            };

            await _templateRepository.AddAsync(template);
            var result = await _templateRepository.SaveChangesAsync();

            return ResponseBaseOnResult<string>(result, HttpStatusCode.Created);

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


        public async Task<ServiceResult<string>> SignRequestAdd(SignRequestAddCommand commad)
        {
            var id = _idGenerator.CreateId();

            var dirPath = PathHelper.CreateTempWorkingDirectory(_applicationOptions.TempFolder, id.ToString());

            var fileName = $"{_applicationOptions.PrefixOptions.Original}{id}.pdf";
            var filePath = await _downloaderService.Download(commad.FileURL, dirPath, fileName);


            var templateIdExist = await _templateRepository.AnyAsync(p => p.Id == commad.TemplateId && !p.Deleted);

            if (!templateIdExist)
            {
                return new ServiceResult<string>(HttpStatusCode.UnprocessableEntity, _applicationErrors.TemplateIdDoesNotExist);
            }

            var signRequest = new SignRequest
            {
                Id = id,
                Email = commad.Email,
                Description = commad.Description,
                ConfirmCode = GenerateConfirmCode(),
                ExpireIn = DateTimeOffset.Now.AddMinutes(_applicationOptions.ConfirmCodeOptions.ExpireInMinutes),
                Done = false,
                TemplateId = commad.TemplateId,
            };

            await _signRequestRepository.AddAsync(signRequest);
            var saveResult = await _signRequestRepository.SaveChangesAsync();

            return ResponseBaseOnResult<string>(saveResult, HttpStatusCode.Created);

        }


        private string GenerateConfirmCode()
        {
            var fullCode = _idGenerator.CreateId().ToString();
            return fullCode.Substring(0, Math.Min(fullCode.Length, _applicationOptions.ConfirmCodeOptions.Length));
        }


        private ServiceResult<T> ResponseBaseOnResult<T>(int result, HttpStatusCode successCode)
        {
            if (result > 0)
            {
                return new ServiceResult<T>(successCode);
            }
            else
            {
                return new ServiceResult<T>(HttpStatusCode.InternalServerError, _applicationErrors.NoRowAffected);
            }
        }
    }
}
