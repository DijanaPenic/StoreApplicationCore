namespace Store.Messaging.Templates.Models.Email
{
    public class ConfirmAccountViewModel
    {
        public ConfirmAccountViewModel(string confirmUrl)
        {
            ConfirmUrl = confirmUrl;
        }

        public string ConfirmUrl { get; }
    }
}