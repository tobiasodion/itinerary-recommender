using System;

namespace az_function
{
    public static class TimestampHelper
    {
        public static string GetFormattedTimestamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}