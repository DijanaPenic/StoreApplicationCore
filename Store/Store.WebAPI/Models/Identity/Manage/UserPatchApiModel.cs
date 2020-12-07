﻿
using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class UserPatchApiModel
    {
        [Required]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters.")]
        public string LastName { get; set; }

        public bool IsApproved { get; set; }

        public string[] Roles { get; set; }
    }
}