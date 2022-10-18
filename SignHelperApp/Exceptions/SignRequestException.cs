using System.Net;

namespace SignHelperApp.Exceptions
{
    public class SignRequestException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public SignRequestException(HttpStatusCode stauts, string message) : base(message)
        {
            StatusCode = stauts;
        }
    }
}
