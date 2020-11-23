using System;

namespace Store.WebAPI.Models.Identity
{
    public class TokenResponseApiModel
    {
        public string ConfirmationToken { get; set; }

        public Guid UserId { get; set; }
    }
}