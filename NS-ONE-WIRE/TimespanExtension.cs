using System;

namespace Common
{
    public static class TimespanExtension
    {
        private const long TicksPerMicroseconds = TimeSpan.TicksPerMillisecond/1000;

        public static long TotalMiliseconds(this TimeSpan @this)
        {
            long result = @this.Ticks/TimeSpan.TicksPerMillisecond;
            return result;
        }

        public static long TotalMicroseconds(this TimeSpan @this)
        {
            long result = @this.Ticks/TicksPerMicroseconds;
            return result;
        }

        public static TimeSpan FromMilliseconds(int ms)
        {
            var result = new TimeSpan(0, 0, 0, 0, ms);
            return result;
        }
    }
}
