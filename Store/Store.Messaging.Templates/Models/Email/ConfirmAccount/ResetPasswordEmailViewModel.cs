﻿namespace Store.Messaging.Templates.Models.Email
{
    public class ResetPasswordEmailViewModel
    {
        public ResetPasswordEmailViewModel(string confirmUrl, string userName)
        {
            ConfirmUrl = confirmUrl;
            UserName = userName;
        }

        public string ConfirmUrl { get; set; }

        public string UserName { get; set; }
    }
}