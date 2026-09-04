using System;

namespace LegacyShop.Services
{
    /// <summary>
    /// В DateTime нет рабочих дней, поэтому досыпаем своё.
    /// </summary>
    public static class DateUtils
    {
        public static DateTime AddBusinessDays(DateTime date, int days)
        {
            DateTime result = date;
            int added = 0;
            while (added < days)
            {
                result = result.AddDays(1);
                if (result.DayOfWeek != DayOfWeek.Saturday && result.DayOfWeek != DayOfWeek.Sunday)
                {
                    added = added + 1;
                }
            }
            return result;
        }

        public static string FormatShort(DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }
    }
}
