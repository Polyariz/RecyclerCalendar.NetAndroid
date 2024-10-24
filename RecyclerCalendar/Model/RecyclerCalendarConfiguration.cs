namespace Com.TejPratapSingh.RecyclerCalendar.Model
{
    public class RecyclerCalendarConfiguration
    {
        public enum CalendarViewType
        {
            Horizontal,
            Vertical
        }

        public enum StartDayOfWeek
        {
            Saturday = 1,
            Sunday = 0,
            Monday = -1
        }

        public CalendarViewType ViewType { get; }
        public Java.Util.Locale CalendarLocale { get; }
        public bool IncludeMonthHeader { get; }
        public StartDayOfWeek WeekStartOffset { get; set; }

        public RecyclerCalendarConfiguration(
            CalendarViewType viewType,
            Java.Util.Locale calendarLocale,
            bool includeMonthHeader)
        {
            ViewType = viewType;
            CalendarLocale = calendarLocale;
            IncludeMonthHeader = includeMonthHeader;
            WeekStartOffset = StartDayOfWeek.Sunday;
        }
    }
}
