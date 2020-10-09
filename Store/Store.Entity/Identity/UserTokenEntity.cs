﻿using System;
using Microsoft.AspNetCore.Identity;

namespace Store.Entities.Identity
{
    public class UserTokenEntity 
    {
        public Guid UserId { get; set; }

        public string LoginProvider { get; set; }

        public string Name { get; set; }

        [ProtectedPersonalData]
        public string Value { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}