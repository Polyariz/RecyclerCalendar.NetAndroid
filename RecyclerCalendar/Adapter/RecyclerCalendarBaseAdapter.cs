using Android.OS;
using AndroidX.RecyclerView.Widget;
using Com.TejPratapSingh.RecyclerCalendar.Model;
using Com.TejPratapSingh.RecyclerCalendar.Utilities;
using Java.Util;

namespace Com.TejPratapSingh.RecyclerCalendar.Adapter
{
    public abstract class RecyclerCalendarBaseAdapter : RecyclerView.Adapter
    {
        private readonly MyHandlerThread _handlerThread;
        private readonly Handler _handler;
        private readonly List<RecyclerCalenderViewItem> _calendarItemList;
        private readonly RecyclerCalendarConfiguration _configuration;

        protected RecyclerCalendarBaseAdapter(DateTime startDate, DateTime endDate, RecyclerCalendarConfiguration configuration)
        {
            _configuration = configuration;
            _calendarItemList = new List<RecyclerCalenderViewItem>();

            _handlerThread = new MyHandlerThread(nameof(RecyclerCalendarBaseAdapter));
            _handler = new Handler(Looper.MainLooper!);

            Calendar startCalendar = Calendar.GetInstance(configuration.CalendarLocale);
            Calendar endCalendar = Calendar.GetInstance(configuration.CalendarLocale);

            InitializeCalendarDates(startCalendar, endCalendar, startDate, endDate, configuration);
            InitializeCalendarItems(startCalendar, endCalendar);
        }

        private void InitializeCalendarDates(Calendar startCalendar, Calendar endCalendar,
            DateTime startDate, DateTime endDate, RecyclerCalendarConfiguration configuration)
        {
            if (configuration.ViewType == RecyclerCalendarConfiguration.CalendarViewType.Horizontal) 
                InitializeHorizontalDates(startCalendar, endCalendar, startDate, endDate); 
            else 
                InitializeVerticalDates(startCalendar, endCalendar, startDate, endDate); 
        }

        private void InitializeHorizontalDates(Calendar startCalendar, Calendar endCalendar, DateTime startDate, DateTime endDate)
        {
            startCalendar.Time = startDate.ToJavaDate();
            int thisDayOfWeek = startCalendar.Get(CalendarField.DayOfWeek);
            thisDayOfWeek += (int)_configuration.WeekStartOffset;

            if (thisDayOfWeek - 1 < 0)
            {
                thisDayOfWeek = 7 + (thisDayOfWeek - 1);
                startCalendar.Add(CalendarField.Date, -1 * thisDayOfWeek);
            }
            else
            {
                startCalendar.Add(CalendarField.Date, -1 * (thisDayOfWeek - 1));
            }

            SetCalendarTime(startCalendar);

            endCalendar.Time = endDate.ToJavaDate();
            endCalendar.Set(CalendarField.DayOfMonth, endCalendar.GetActualMaximum(CalendarField.DayOfMonth));
            SetCalendarTime(endCalendar);
        }

        private void InitializeVerticalDates(Calendar startCalendar, Calendar endCalendar, DateTime startDate, DateTime endDate)
        {
            startCalendar.Time = startDate.ToJavaDate();
            startCalendar.Set(CalendarField.DayOfMonth, 1);
            SetCalendarTime(startCalendar);

            endCalendar.Time = endDate.ToJavaDate();
            endCalendar.Set(CalendarField.DayOfMonth, endCalendar.GetActualMaximum(CalendarField.DayOfMonth));
            SetCalendarTime(endCalendar);
        }

        private void SetCalendarTime(Calendar calendar)
        {
            calendar.Set(CalendarField.HourOfDay, 0);
            calendar.Set(CalendarField.Minute, 0);
            calendar.Set(CalendarField.Second, 0);
            calendar.Set(CalendarField.Millisecond, 0);
        }

        private void InitializeCalendarItems(Calendar startCalendar, Calendar endCalendar)
        {
            Action runnable = () =>
            {
                while (startCalendar.Time.CompareTo(endCalendar.Time) <= 0)
                {
                    ProcessCalendarDay(startCalendar);
                    startCalendar.Add(CalendarField.Date, 1);
                }
                _handler.Post(() => NotifyDataSetChanged());
            };

            _handlerThread.Start();
            _handlerThread.PrepareHandler();
            _handlerThread.PostTask(runnable);
        }

        private void ProcessCalendarDay(Calendar calendar)
        {
            int dayOfMonth = calendar.Get(CalendarField.DayOfMonth);
            int dayOfWeek = calendar.Get(CalendarField.DayOfWeek);
            dayOfWeek += (int)_configuration.WeekStartOffset;

            if (_configuration.ViewType == RecyclerCalendarConfiguration.CalendarViewType.Vertical && dayOfMonth == 1)
            {
                ProcessMonthStart(calendar, dayOfWeek);
            }

            var calendarDate = calendar.Time.ToDateTime();
            _calendarItemList.Add(new RecyclerCalenderViewItem(calendarDate, 1, false, false));
        }

        private void ProcessMonthStart(Calendar calendar, int dayOfWeek)
        {
            if (_configuration.IncludeMonthHeader)
            {
                _calendarItemList.Add(new RecyclerCalenderViewItem(
                    calendar.Time.ToDateTime(),
                    7,
                    false,
                    true
                ));
            }

            int spanSize = dayOfWeek - 1;
            if (spanSize < 0)
            {
                spanSize = 7 + spanSize;
            }

            if (spanSize > 0)
            {
                _calendarItemList.Add(new RecyclerCalenderViewItem(
                    calendar.Time.ToDateTime(),
                    spanSize,
                    true,
                    false
                ));
            }
        }

        public override int ItemCount => _calendarItemList.Count;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            OnBindViewHolder(holder, position, _calendarItemList[position]);
        }

        public abstract void OnBindViewHolder(RecyclerView.ViewHolder holder, int position, RecyclerCalenderViewItem calendarItem);

        public RecyclerCalenderViewItem? GetItem(int position) =>
            position < _calendarItemList.Count ? _calendarItemList[position] : null;
        /**
        * Set LayoutManager of recycler view to GridLayoutManager with span of 7 (week)
        */
        public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
        {
            base.OnAttachedToRecyclerView(recyclerView);

            if (_configuration.ViewType == RecyclerCalendarConfiguration.CalendarViewType.Horizontal)
            {
                recyclerView.SetLayoutManager(new LinearLayoutManager(recyclerView.Context, LinearLayoutManager.Horizontal, false));
                var snapHelper = new PagerSnapHelper();
                recyclerView.SetOnFlingListener(null);
                snapHelper.AttachToRecyclerView(recyclerView);
            }
            else
            {
                var gridLayoutManager = new GridLayoutManager(recyclerView.Context, 7);
                gridLayoutManager.SetSpanSizeLookup(new CustomSpanSizeLookup(this));
                recyclerView.SetLayoutManager(gridLayoutManager);
            }
        }

        public override void OnDetachedFromRecyclerView(RecyclerView recyclerView)
        {
            base.OnDetachedFromRecyclerView(recyclerView);
            _handlerThread.Quit();
        }

        private class CustomSpanSizeLookup : GridLayoutManager.SpanSizeLookup
        {
            private readonly RecyclerCalendarBaseAdapter _adapter;

            public CustomSpanSizeLookup(RecyclerCalendarBaseAdapter adapter)
            {
                _adapter = adapter;
            }

            public override int GetSpanSize(int position) =>
                _adapter.GetItem(position)?.SpanSize ?? 0;
        }

        public class MyHandlerThread : HandlerThread
        {
            private Handler _handler = null!;

            public MyHandlerThread(string name) : base(name) { }

            public void PostTask(Action task)
            {
                _handler!.Post(task);
            }

            public void PrepareHandler()
            {
                _handler = new Handler(Looper!);
            }
        }
    }
}