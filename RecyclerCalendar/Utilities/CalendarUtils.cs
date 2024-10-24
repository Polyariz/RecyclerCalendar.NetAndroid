using Java.Text;
using Java.Util;

namespace Com.TejPratapSingh.RecyclerCalendar.Utilities
{
    /// <summary>
    /// Utility class for calendar date formatting and parsing operations
    /// </summary>
    public static class CalendarUtils
    {
        public const string DbDateFormat = "yyyyMMdd";
        public const string DbYearMonthFormat = "yyyyMM";
        public const string DbDateFormatWithTime = "yyyyMMddHHmm";
        public const string ShortDateFormat = "dd-MMM-yyyy";
        public const string LongDateFormat = "EEE, dd MMM yyyy";
        public const string LongDateDay = "E";
        public const string DisplayTimeFormat = "hh:mm aaa";
        public const string DisplayDateTimeFormat = "EEE, dd MMM yyyy, hh:mm aaa";
        public const string DisplayWeekDayFormat = "EEEEEE";
        public const string DisplayMonthFormat = "MMMM";
        public const string DisplayDateFormat = "dd";

        /// <summary>
        /// Returns DateTime Object from date string and date format
        /// </summary>
        public static DateTime DateFromAnyFormat(Locale locale, string date, string format)
        {
            try
            {
                var formatter = new SimpleDateFormat(format, locale);
                var javaDate = formatter.Parse(date)!;
                return javaDate.ToDateTime();
            }
            catch (ParseException)
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Returns formatted date string using specified format and locale
        /// </summary>
        public static string DateStringFromFormat(Locale locale, DateTime date, string format)
        {
            try
            {
                var formatter = new SimpleDateFormat(format, locale);
                var javaDate = date.ToJavaDate();
                return formatter.Format(javaDate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets formatted GMT string representation of date
        /// </summary>
        public static string GetGmt(DateTime date)
        {
            var dfDate = new SimpleDateFormat("dd/MMM/yyyy HH:mm:ss", Locale.Default);
            var javaDate = date.ToJavaDate();
            return dfDate.Format(javaDate);
        }

        /// <summary>
        /// Extension method to convert Java.Util.Date to DateTime
        /// </summary>
        public static DateTime ToDateTime(this Date date)
        {
            var calendar = Calendar.Instance;
            calendar.Time = date;
            return new DateTime(
                calendar.Get(CalendarField.Year),
                calendar.Get(CalendarField.Month) + 1,
                calendar.Get(CalendarField.DayOfMonth),
                calendar.Get(CalendarField.HourOfDay),
                calendar.Get(CalendarField.Minute),
                calendar.Get(CalendarField.Second),
                calendar.Get(CalendarField.Millisecond),
                DateTimeKind.Local);
        }

        /// <summary>
        /// Extension method to convert DateTime to Java.Util.Date
        /// </summary>
        public static Date ToJavaDate(this DateTime dateTime)
        {
            var calendar = Calendar.Instance;
            calendar.Set(
                dateTime.Year,
                dateTime.Month - 1,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second);
            calendar.Set(CalendarField.Millisecond, dateTime.Millisecond);
            return calendar.Time;
        }
    }
}