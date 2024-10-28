using Android.Util;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Com.TejPratapSingh.RecyclerCalendar.Model;
using Com.TejPratapSingh.RecyclerCalendar.Views;

namespace Com.TejPratapSingh.RecyclerCalendar.Adapter
{
    public class InfiniteRecyclerCalendarAdapter : RecyclerView.Adapter
    {
        private const string TAG = "InfiniteRecyclerCalendar";

        private readonly OnDateSelected _dateSelectListener;
        private readonly InfiniteRecyclerCalendarConfiguration _configuration;
        private readonly RecyclerView.RecycledViewPool _viewPool;
        private SnapHelper _snapHelper;

        public interface OnDateSelected
        {
            void OnDateSelected(DateTime date);
        }

        public InfiniteRecyclerCalendarAdapter(OnDateSelected dateSelectListener, InfiniteRecyclerCalendarConfiguration configuration)
        {
            _dateSelectListener = dateSelectListener;
            _configuration = configuration;
            _viewPool = new RecyclerView.RecycledViewPool();
            _snapHelper = new PagerSnapHelper();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context)!
                .Inflate(Resource.Layout.item_infinite_calendar, parent, false)!;
            return new InfiniteViewHolder(view);
        }

        public override int ItemCount => int.MaxValue;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            int midPosition = int.MaxValue / 2;
            int currentMonth = position - midPosition;

            var currentDate = DateTime.Now;
            var startDate = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(currentMonth);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Convert InfiniteRecyclerCalendarConfiguration TO SimpleRecyclerCalendarConfiguration
            SimpleRecyclerCalendarConfiguration.SelectionMode selectionMode =
                _configuration.CurrentSelectionMode is InfiniteRecyclerCalendarConfiguration.SelectionModeNone
                ? new SimpleRecyclerCalendarConfiguration.SelectionModeNone()
                : new SimpleRecyclerCalendarConfiguration.SelectionModeNone();

            var simpleRecyclerCalendarConfiguration = new SimpleRecyclerCalendarConfiguration(
                calenderViewType: RecyclerCalendarConfiguration.CalendarViewType.Vertical,
                calendarLocale: _configuration.CalendarLocale,
                includeMonthHeader: _configuration.IncludeMonthHeader,
                selectionMode: selectionMode
            );

            simpleRecyclerCalendarConfiguration.WeekStartOffset = _configuration.WeekStartOffset;

            var infiniteViewHolder = (InfiniteViewHolder)holder;
            infiniteViewHolder.SimpleRecyclerCalendarView.SetRecycledViewPool(_viewPool);
            infiniteViewHolder.SimpleRecyclerCalendarView.Initialise(
                startDate: startDate,
                endDate: endDate,
                configuration: simpleRecyclerCalendarConfiguration,
                dateSelectListener: new SimpleRecyclerCalendarAdapterDateSelectedListener(
                    date =>
                    {
                        string currentDateString = date.ToString("yyyyMMdd", new System.Globalization.CultureInfo(_configuration.CalendarLocale.ToString()!));
                        Log.Debug(TAG, $"OnDateSelected: {currentDateString}");
                        _dateSelectListener.OnDateSelected(date);
                    }
                )
            );
        }
        /**
        * Set LayoutManager of recycler view to GridLayoutManager with span of 7 (week)
        */
        public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
        {
            base.OnAttachedToRecyclerView(recyclerView);

            if (_configuration.ViewType == RecyclerCalendarConfiguration.CalendarViewType.Horizontal)
            {
                recyclerView.SetLayoutManager(new LinearLayoutManager(recyclerView.Context, LinearLayoutManager.Horizontal, false));
                recyclerView.SetOnFlingListener(null);
                _snapHelper = new PagerSnapHelper();
                _snapHelper.AttachToRecyclerView(recyclerView);
            }
            else
            {
                recyclerView.SetLayoutManager(new LinearLayoutManager(recyclerView.Context, LinearLayoutManager.Vertical, false));
                recyclerView.SetOnFlingListener(null);
                _snapHelper.AttachToRecyclerView(null);
            }
        }

        public class InfiniteViewHolder : RecyclerView.ViewHolder
        {
            public SimpleRecyclerCalendarView SimpleRecyclerCalendarView { get; }

            public InfiniteViewHolder(View itemView) : base(itemView)
            {
                SimpleRecyclerCalendarView = itemView.FindViewById<SimpleRecyclerCalendarView>(Resource.Id.simpleCalenderRecyclerView);
            }
        }

        private class SimpleRecyclerCalendarAdapterDateSelectedListener : Java.Lang.Object, SimpleRecyclerCalendarAdapter.IOnDateSelected
        {
            private readonly Action<DateTime> _onDateSelected;

            public SimpleRecyclerCalendarAdapterDateSelectedListener(Action<DateTime> onDateSelected)
            {
                _onDateSelected = onDateSelected;
            }
            public void OnDateSelected(DateTime date)
            {
                _onDateSelected(date);
            }
        }
    }
}