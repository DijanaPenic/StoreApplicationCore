namespace Store.EmailTemplate.Models
{
    public class ChangePasswordEmailViewModel
    {
        public ChangePasswordEmailViewModel(string userName, string newPassword)
        {
            UserName = userName;
            NewPassword = newPassword;
        }

        public string UserName { get; set; }

        public string NewPassword { get; set; }
    }
}