using System;

namespace Store.WebAPI.Models.Identity
{
    public class UserLoginGetApiModel
    {
        public string ProviderDisplayName { get; set; }
         
        public string Token { get; set; }
         
        public bool IsConfirmed { get; set; }
         
        public DateTime DateCreatedUtc { get; set; }
         
        public DateTime DateUpdatedUtc { get; set; }
    }
}