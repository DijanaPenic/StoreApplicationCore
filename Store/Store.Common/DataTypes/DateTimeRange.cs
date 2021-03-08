using System;

namespace Store.Common.DataTypes
{
    public struct DateTimeRange
    {
        private readonly DateTime? _max;
        private readonly DateTime? _min;

        public DateTime? Max { get { return _max; } }

        public DateTime? Min { get { return _min; } }

        public DateTimeRange(DateTime? min, DateTime? max)
        {
            if (min.HasValue && max.HasValue && min.Value > max.Value)
            {
                throw new ArgumentOutOfRangeException("Minimum value is larger than maximum value.");
            }

            _min = min;
            _max = max;
        }

        public bool IsBetween(DateTime dateTime)
        {
            DateTime localMin = _min ?? DateTime.MinValue;
            DateTime localMax = _max ?? DateTime.MaxValue;

            return localMin <= dateTime && dateTime <= localMax;
        }
    }
}