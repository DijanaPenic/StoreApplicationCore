﻿namespace Store.Common.Parameters.Options
{
    public class OptionsParameters : IOptionsParameters
    {
        public string[] Properties { get; set; }

        public OptionsParameters(string[] properties = null)
        {
            Properties = properties;
        }
    }
}