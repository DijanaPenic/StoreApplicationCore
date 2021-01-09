namespace Store.Messaging.Templates.Models.Email
{
    public class ChangePasswordViewModel
    {
        public ChangePasswordViewModel(string userName, string newPassword)
        {
            UserName = userName;
            NewPassword = newPassword;
        }

        public string UserName { get; set; }

        public string NewPassword { get; set; }
    }
}