using System;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Store.Common.Helpers
{
    public static class GuidHelper
    {
        private static SequentialGuidValueGenerator _sequentialGuidGenerator;

        public static SequentialGuidValueGenerator SequentialGuidGenerator
        {
            get { return _sequentialGuidGenerator; }

            set
            {
                if (value == null)
                {
                    _sequentialGuidGenerator = new SequentialGuidValueGenerator();
                }
            }
        }

        public static Guid NewSequentialGuid() => SequentialGuidGenerator.Next(null);

        public static bool IsNullOrEmpty(object value)
        {
            return ((value as Guid?).GetValueOrDefault() == Guid.Empty);
        }

        public static Guid Get(string value)
        {
            Guid.TryParse(value, out Guid result);

            return result;
        }
    }
}