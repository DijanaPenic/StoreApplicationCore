﻿using System.Text.Json;

using Store.Common.Extensions;

namespace Store.WebAPI.Application.Startup.Policies
{
    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToSnakeCase();
        }
    }
}