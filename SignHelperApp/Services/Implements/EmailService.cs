using Microsoft.Extensions.Options;
using SignHelperApp.Options;
using SignHelperApp.Services.Interfaces;
using SignHelperApp.Types;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace SignHelperApp.Services.Implements
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationOptions _applicationOptions;
        private readonly EmailServiceOptions _emailServiceOptions;

        public EmailService(HttpClient httpClient,
            IOptions<ApplicationOptions> applicationOptions,
            IOptions<EmailServiceOptions> emailServiceOptions)
        {
            _httpClient = httpClient;
            _applicationOptions = applicationOptions.Value;
            _emailServiceOptions = emailServiceOptions.Value;
        }

        public async Task SendEmail(Email email)
        {
            _httpClient.DefaultRequestHeaders.Add("Token", _emailServiceOptions.Token);
            _httpClient.DefaultRequestHeaders.Add("CallerID", _emailServiceOptions.CallerId);
            _httpClient.Timeout = TimeSpan.FromSeconds(120);

            var toSendEmailJson = new StringContent(
            JsonSerializer.Serialize(email),
            Encoding.UTF8,
            Application.Json);


            var response = await _httpClient.PostAsync(_emailServiceOptions.Url, toSendEmailJson);
            response.EnsureSuccessStatusCode();
        }
    }
}
