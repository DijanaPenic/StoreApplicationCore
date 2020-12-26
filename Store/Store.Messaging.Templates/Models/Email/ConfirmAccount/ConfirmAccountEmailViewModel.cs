namespace Store.Messaging.Templates.Models.Email
{
    public class ConfirmAccountEmailViewModel
    {
        public ConfirmAccountEmailViewModel(string confirmUrl)
        {
            ConfirmUrl = confirmUrl;
        }

        public string ConfirmUrl { get; set; }
    }
}