using System;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Store.Common.Helpers
{
    public static class GuidHelper
    {
        private static SequentialGuidValueGenerator _sequentialGuidGenerator;

        public static SequentialGuidValueGenerator SequentialGuidGenerator
        {
            get
            {
                _sequentialGuidGenerator ??= new SequentialGuidValueGenerator();

                return _sequentialGuidGenerator;
            }

            set
            {
                _sequentialGuidGenerator = value;
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