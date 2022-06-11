using System;

namespace Utilities.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToAppFormat(this DateTime value) => value.ToString("dd/MM/yyyy hh:mm:ss");
    }
}