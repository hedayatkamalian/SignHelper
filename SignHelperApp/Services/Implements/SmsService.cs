using Microsoft.Extensions.Options;
using SignHelperApp.Options;
using SignHelperApp.Services.Interfaces;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace SignHelperApp.Services.Implements
{
    public class SmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly SmsOptions _smsOptions;

        public SmsService(HttpClient httpClient, IOptions<SmsOptions> smsOptions)
        {
            _httpClient = httpClient;
            _smsOptions = smsOptions.Value;
        }

        public async Task SendMessage(string phoneNumber, string message)
        {
            _httpClient.DefaultRequestHeaders.Add("Token", _smsOptions.Token);
            _httpClient.DefaultRequestHeaders.Add("CallerID", _smsOptions.CallerId);

            var toSendMessage = new SmsMessage { PhoneNumber = phoneNumber, Message = message };
            var toSendMessageJson = new StringContent(
            JsonSerializer.Serialize(toSendMessage),
            Encoding.UTF8,
            Application.Json);


            var responseMessage = await _httpClient.PostAsync(_smsOptions.Url, toSendMessageJson);
            responseMessage.EnsureSuccessStatusCode();
        }

        public async Task SendTemplateMessage(string phoneNumber, string templateName, Dictionary<string, string>? paramteres)
        {
            var template = _smsOptions.Templates.First(p => p.Name == templateName);

            if (template is null || string.IsNullOrEmpty(template.Text))
            {
                throw new Exception("template or template text is null");
            }

            var templateText = template.Text;

            if (paramteres is not null)
            {
                foreach (var keyWord in paramteres)
                {
                    templateText = templateText.Replace("{" + keyWord.Key.ToLower() + "}", keyWord.Value);
                }
            }


            await SendMessage(phoneNumber, templateText);
        }

    }

    public class SmsMessage
    {
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
