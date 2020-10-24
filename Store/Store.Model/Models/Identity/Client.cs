﻿using System;

using Store.Common.Enums;
using Store.Model.Common.Models.Identity;

namespace Store.Models.Identity
{
    public class Client : IClient
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Secret { get; set; }

        public string Description { get; set; }

        public ApplicationType ApplicationType { get; set; }

        public bool Active { get; set; }

        public int AccessTokenLifeTime { get; set; }

        public int RefreshTokenLifeTime { get; set; }

        public string AllowedOrigin { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}
