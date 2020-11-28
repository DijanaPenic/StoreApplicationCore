﻿using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class AuthenticateTwoFactorRequestApiModel
    {
        [Required]
        [StringLength(6, ErrorMessage = "The {0} must be {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        public string Code { get; set; }

        public bool UseRecoveryCode { get; set; }

        [Required]
        public string ClientId { get; set; }
    }
}