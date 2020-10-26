namespace Store.WebAPI.Models
{
    public class ClientAuthResult
    {
        public bool Succeeded { get; private set; }

        public string ErrorMessage { get; private set; }

        public ClientAuthResult()
        {
            Succeeded = true;
        }

        public ClientAuthResult(string errorMessage)
        {
            Succeeded = false;
            ErrorMessage = errorMessage;
        }
    }
}