using System;

namespace Station
{
    public class DateTimeUtils
    {
        private static readonly DateTime UnixDateTime = new DateTime(1970, 1, 1);

        public static long GetEpochTimeInMs()
        {
            TimeSpan timeSpan = DateTime.UtcNow.Subtract(UnixDateTime);
            long totalMilliseconds = (long) timeSpan.TotalMilliseconds;
            return totalMilliseconds;
        }

        public static long GetEpochTimeInSeconds()
        {
            TimeSpan timeSpan = DateTime.UtcNow.Subtract(UnixDateTime);
            long totalSeconds = (long)timeSpan.TotalSeconds;
            return totalSeconds;
        }

        public static long DateTimeToMilliSeconds(DateTime dateTime)
        {
            TimeSpan timeSpan = dateTime.Subtract(DateTime.MinValue);
            long totalMilliseconds = (long) timeSpan.TotalMilliseconds;
            return totalMilliseconds;
        }

        public static long DateTimeToSeconds(DateTime dateTime)
        {
            TimeSpan timeSpan = dateTime.Subtract(DateTime.MinValue);
            long totalSeconds = (long)timeSpan.TotalSeconds;
            return totalSeconds;
        }

        public static DateTime MillisecondsToDateTime(long timeMs)
        {
            return DateTime.MinValue.AddMilliseconds(timeMs);
        }
        
        public static long CurrentTimeInMilliseconds()
        {
            return DateTimeToMilliSeconds(DateTime.UtcNow);
        }

        public static string CurrentTimeString()
        {
            return DateTime.Now.ToString("h:mm:ss");
        }

        public static string CurrentDate()
        {
            return DateTime.Now.ToString("MM/dd/yyyy");
        }

        public static string CurrentDateTime()
        {
            return DateTime.Now.ToString("MM/dd/yyyy h:mm:ss");
        }

        public static string CurrentDateTimeWithMS()
        {
            return DateTime.Now.ToString("MMMM dd HH:mm:ss:ff");
        }

        public static string CurrentDay()
        {
            return DateTime.Now.DayOfWeek.ToString();
        }

        public static string TicksToTime(int tick)
        {
            int ticktoMs = tick * 16;
            TimeSpan t = TimeSpan.FromMilliseconds(ticktoMs);
            return string.Format("{0:D2}m:{1:D2}s:{2:D3}ms - {3} tick",
                t.Minutes,
                t.Seconds,
                t.Milliseconds,
                tick);
        }
    }
}