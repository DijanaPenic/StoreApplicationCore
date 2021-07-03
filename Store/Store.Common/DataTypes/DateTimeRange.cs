using System;

namespace Store.Common.DataTypes
{
    public readonly struct DateTimeRange
    {
        public DateTime? Max { get; }

        public DateTime? Min { get; }

        public DateTimeRange(DateTime? min, DateTime? max)
        {
            if (min.HasValue && max.HasValue && min.Value > max.Value)
            {
                throw new ArgumentOutOfRangeException("Minimum value is larger than maximum value.");
            }

            Min = min;
            Max = max;
        }

        public bool IsBetween(DateTime dateTime)
        {
            DateTime localMin = Min ?? DateTime.MinValue;
            DateTime localMax = Max ?? DateTime.MaxValue;

            return localMin <= dateTime && dateTime <= localMax;
        }
    }
}