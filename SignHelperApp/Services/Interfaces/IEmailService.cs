using SignHelperApp.Types;

namespace SignHelperApp.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(Email email);
    }
}
