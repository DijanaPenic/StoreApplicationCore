﻿using System;

namespace Store.WebAPI.Models.Identity
{
    public class RoleGetApiModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Stackable { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public PolicyGetApiModel[] Policies { get; set; }
    }
}