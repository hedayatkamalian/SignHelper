namespace SignHelperApp.Services.Interfaces
{
    public interface ISmsService
    {
        Task SendMessage(string phoneNumber, string message);
        Task SendTemplateMessage(string phoneNumber, string templateName, Dictionary<string, string>? keyWords);
    }
}
