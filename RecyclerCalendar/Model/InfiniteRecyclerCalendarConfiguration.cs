namespace Com.TejPratapSingh.RecyclerCalendar.Model
{
    /// <summary>
    /// Configuration for infinite recycler calendar
    /// </summary>
    public class InfiniteRecyclerCalendarConfiguration : RecyclerCalendarConfiguration
    {
        /// <summary>
        /// Gets or sets current selection mode
        /// </summary>
        public SelectionMode CurrentSelectionMode { get; set; }

        /// <summary>
        /// Creates new instance of InfiniteRecyclerCalendarConfiguration
        /// </summary>
        public InfiniteRecyclerCalendarConfiguration(
            CalendarViewType viewType,
            Java.Util.Locale calendarLocale,
            bool includeMonthHeader,
            SelectionMode selectionMode)
            : base(viewType, calendarLocale, includeMonthHeader)
        {
            CurrentSelectionMode = selectionMode;
        }

        /// <summary>
        /// Base class for calendar selection modes
        /// </summary>
        public abstract class SelectionMode
        {
            public enum Type
            {
                None
            }

            public Type SelectionType { get; set; } = Type.None;
        }

        /// <summary>
        /// Selection mode with no selection
        /// </summary>
        public sealed class SelectionModeNone : SelectionMode
        {
            public SelectionModeNone()
            {
                SelectionType = Type.None;
            }
        }
    }
}