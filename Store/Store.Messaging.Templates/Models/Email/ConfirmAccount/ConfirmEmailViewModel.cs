namespace Store.Messaging.Templates.Models.Email
{
    public class ConfirmEmailViewModel
    {
        public ConfirmEmailViewModel(string userName, string confirmUrl)
        {
            UserName = userName;
            ConfirmUrl = confirmUrl;
        }

        public string UserName { get; set; }

        public string ConfirmUrl { get; set; }
    }
}