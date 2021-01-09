namespace Store.Messaging.Templates.Models.Email
{
    public class ResetPasswordViewModel
    {
        public ResetPasswordViewModel(string confirmUrl, string userName)
        {
            ConfirmUrl = confirmUrl;
            UserName = userName;
        }

        public string ConfirmUrl { get; set; }

        public string UserName { get; set; }
    }
}