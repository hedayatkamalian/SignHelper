using MustIt.Kernel.Services;
using SignHelperApp.Commands.SignRequests;
using SignHelperApp.Commands.Templates;
using SignHelperApp.Entities;

namespace SignHelperApp.Services.Interfaces
{
    public interface ISignHelperService
    {
        Task<ServiceResult<string>> TemplateAdd(TemplateAddCommand command);
        Task<ServiceResult<string>> TemplateDelete(long id);
        Task<ServiceResult<IList<Template>>> TemplateGetAll();
        Task<ServiceResult<string>> SignRequestAdd(SignRequestAddCommand commad);
        Task<ServiceResult<string>> SingRequestSign(long id, string confirmCode);
    }
}