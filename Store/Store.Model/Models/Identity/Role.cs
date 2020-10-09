﻿using System;

using Store.Model.Common.Models.Identity;

namespace Store.Model.Models.Identity
{
    public class Role : IRole
    {
        public Guid Id { get; set; }

        public string ConcurrencyStamp { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public bool Stackable { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}