namespace Com.TejPratapSingh.RecyclerCalendar.Model
{
    /// <summary>
    /// Configuration for simple recycler calendar
    /// </summary>
    public class SimpleRecyclerCalendarConfiguration : RecyclerCalendarConfiguration
    {
        /// <summary>
        /// Gets or sets current selection mode
        /// </summary>
        public SelectionMode CurrentSelectionMode { get; set; }

        public SimpleRecyclerCalendarConfiguration(
            CalendarViewType calenderViewType,
            Java.Util.Locale calendarLocale,
            bool includeMonthHeader,
            SelectionMode selectionMode)
            : base(calenderViewType, calendarLocale, includeMonthHeader)
        {
            CurrentSelectionMode = selectionMode;
        }

        public abstract class SelectionMode
        {
            public enum Type
            {
                None,
                Single,
                Multiple,
                Range
            }

            public Type SelectionType { get; set; } = Type.None;
        }

        public sealed class SelectionModeNone : SelectionMode
        {
            public SelectionModeNone()
            {
                SelectionType = Type.None;
            }
        }

        public sealed class SelectionModeSingle : SelectionMode
        {
            public DateTime SelectedDate { get; set; }

            public SelectionModeSingle(DateTime? selectedDate = null)
            {
                SelectionType = Type.Single;
                SelectedDate = selectedDate ?? DateTime.Now;
            }
        }

        public sealed class SelectionModeMultiple : SelectionMode
        {
            public Dictionary<string, DateTime> SelectionStartDateList { get; set; }

            public SelectionModeMultiple(Dictionary<string, DateTime> selectionStartDateList)
            {
                SelectionType = Type.Multiple;
                SelectionStartDateList = selectionStartDateList ?? new Dictionary<string, DateTime>();
            }
        }

        public sealed class SelectionModeRange : SelectionMode
        {
            public DateTime SelectionStartDate { get; set; }
            public DateTime SelectionEndDate { get; set; }

            public SelectionModeRange(DateTime selectionStartDate, DateTime selectionEndDate)
            {
                SelectionType = Type.Range;
                SelectionStartDate = selectionStartDate;
                SelectionEndDate = selectionEndDate;
            }
        }
    }
}