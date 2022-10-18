namespace SignHelperApp.Commands.SignRequests
{
    public class SignRequestSignCommand
    {
        public SignRequestSignCommand(long id, string confirmCode, string? signImageData)
        {
            Id = id;
            ConfirmCode = confirmCode;
            SignImageData = signImageData.Replace("data:image/png;base64,", "");
        }

        public long Id { get; private set; }
        public string ConfirmCode { get; private set; }
        public string? SignImageData { get; private set; }
    }
}
