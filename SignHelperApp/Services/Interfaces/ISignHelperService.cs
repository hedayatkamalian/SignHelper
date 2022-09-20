using MustIt.Kernel.Services;
using SignHelperApp.Command.SignRequest;
using SignHelperApp.Command.Template;
using SignHelperApp.Entities;

namespace SignHelperApp.Services.Interfaces
{
    public interface ISignHelperService
    {
        Task<ServiceResult<string>> TemplateAdd(TemplateAddCommand command);
        Task<ServiceResult<string>> TemplateDelete(long id);
        Task<ServiceResult<IList<Template>>> TemplateGetAll();
        Task<ServiceResult<string>> SignRequestAdd(SignRequestAddCommand commad);
    }
}