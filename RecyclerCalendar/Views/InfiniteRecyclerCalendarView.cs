using Android.Content;
using Android.Util;
using AndroidX.RecyclerView.Widget;
using Com.TejPratapSingh.RecyclerCalendar.Adapter;
using Com.TejPratapSingh.RecyclerCalendar.Model;

namespace Com.TejPratapSingh.RecyclerCalendar.Views
{
    /// <summary>
    /// Custom RecyclerView implementation for infinite scrolling calendar
    /// </summary>
    public class InfiniteRecyclerCalendarView : RecyclerView
    {
        private const int DEFAULT_CACHE_SIZE = 10;
        private const int DEFAULT_SCROLL_POSITION = int.MaxValue / 2;

        public InfiniteRecyclerCalendarView(Context context) : base(context)
        {
            InitializeView();
        }

        public InfiniteRecyclerCalendarView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitializeView();
        }

        public InfiniteRecyclerCalendarView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitializeView();
        }

        private void InitializeView()
        {
            SetItemViewCacheSize(DEFAULT_CACHE_SIZE);
            HasFixedSize = true;
        }

        /// <summary>
        /// Initializes the calendar view with configuration and date selection listener
        /// </summary>
        /// <param name="configuration">Calendar configuration settings</param>
        /// <param name="dateSelectListener">Callback for date selection events</param>
        public void Initialize(
            InfiniteRecyclerCalendarConfiguration configuration,
            InfiniteRecyclerCalendarAdapter.OnDateSelected dateSelectListener)
        {
            var adapter = new InfiniteRecyclerCalendarAdapter(
                dateSelectListener,
                configuration
            );

            SetAdapter(adapter);
            ScrollToPosition(DEFAULT_SCROLL_POSITION);
        }
    }
}