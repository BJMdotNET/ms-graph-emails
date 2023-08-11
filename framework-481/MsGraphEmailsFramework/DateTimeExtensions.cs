using System;

namespace MsGraphEmailsFramework
{
    public static class DateTimeExtensions
    {
        public static string ToDdMmYyyyDashHhMm(this DateTime dateTime)
        {
            return $"{dateTime.ToDdMmYyyySlashed()} - {dateTime.ToReadableHoursMinutes()}";
        }

        private static string ToDdMmYyyySlashed(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }

        private static string ToReadableHoursMinutes(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm");
        }
    }
}
