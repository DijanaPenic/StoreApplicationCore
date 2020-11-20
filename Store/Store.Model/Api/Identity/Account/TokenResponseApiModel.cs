using System;

namespace Store.Models.Api.Identity
{
    public class TokenResponseApiModel
    {
        public string ConfirmationToken { get; set; }

        public Guid UserId { get; set; }
    }
}