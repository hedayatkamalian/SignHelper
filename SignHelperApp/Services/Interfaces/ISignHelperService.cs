using MustIt.Kernel.Services;
using SignHelperApp.Commands.SignRequests;
using SignHelperApp.Commands.Templates;
using SignHelperApp.DTO;
using SignHelperApp.Entities;

namespace SignHelperApp.Services.Interfaces
{
    public interface ISignHelperService
    {
        Task<ServiceResult<string>> TemplateAdd(TemplateAddCommand command);
        Task<ServiceResult<string>> TemplateDelete(long id);
        Task<ServiceResult<IList<Template>>> TemplateGetAll();
        Task<ServiceResult<string>> SignRequestAdd(SignRequestAddCommand commad);
        Task<ServiceResult<SignRequestDto>> SignRequestGet(long id);
        Task<ServiceResult<SignRequestDto>> SignRequestGetWithCode(long id, string code);
        Task<ServiceResult<string>> SingRequestSign(SignRequestSignCommand command);
        Task<ServiceResult<string>> SingRequestConfirm(long id, string confirmCode);
        Task<ServiceResult<SignRequestDto>> SignRequestSendConfirmCode(long id);
    }
}