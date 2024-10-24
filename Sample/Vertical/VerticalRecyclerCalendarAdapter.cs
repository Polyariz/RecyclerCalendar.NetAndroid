using Android.Views;
using AndroidX.RecyclerView.Widget;
using Com.TejPratapSingh.RecyclerCalendar.Adapter;
using Com.TejPratapSingh.RecyclerCalendar.Model;
using Sample.Model;

namespace Sample.Vertical
{
    public class VerticalRecyclerCalendarAdapter : RecyclerCalendarBaseAdapter
    {
        public interface IOnDateSelected
        {
            void OnDateSelected(DateTime date, SimpleEvent? @event);
        }

        private readonly RecyclerCalendarConfiguration _configuration;
        private readonly Dictionary<int, SimpleEvent> _eventMap;
        private readonly IOnDateSelected _dateSelectListener;

        public VerticalRecyclerCalendarAdapter(
            DateTime startDate,
            DateTime endDate,
            RecyclerCalendarConfiguration configuration,
            Dictionary<int, SimpleEvent> eventMap,
            IOnDateSelected dateSelectListener) : base(startDate, endDate, configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _eventMap = eventMap ?? throw new ArgumentNullException(nameof(eventMap));
            _dateSelectListener = dateSelectListener ?? throw new ArgumentNullException(nameof(dateSelectListener));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context)!
                .Inflate(Resource.Layout.item_calendar_vertical, parent, false)!;
            return new MonthCalendarViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position, RecyclerCalenderViewItem calendarItem)
        {
            if (holder is not MonthCalendarViewHolder monthViewHolder)
                throw new ArgumentException("Incorrect ViewHolder type", nameof(holder));

            InitializeViewHolder(monthViewHolder);

            if (calendarItem.IsHeader)
            {
                BindHeaderItem(monthViewHolder, calendarItem);
            }
            else if (calendarItem.IsEmpty)
            {
                BindEmptyItem(monthViewHolder);
            }
            else
            {
                BindCalendarItem(monthViewHolder, calendarItem);
            }
        }

        private static void InitializeViewHolder(MonthCalendarViewHolder viewHolder)
        {
            viewHolder.ItemView.Visibility = ViewStates.Visible;
            viewHolder.ViewEvent.Visibility = ViewStates.Gone;
            viewHolder.ItemView.SetOnClickListener(null);
        }

        private void BindHeaderItem(MonthCalendarViewHolder viewHolder, RecyclerCalenderViewItem calendarItem)
        {
            string month = calendarItem.Date.ToString("MMMM", new System.Globalization.CultureInfo(_configuration.CalendarLocale.ToString()!));
            int year = calendarItem.Date.Year;

            viewHolder.TextViewDay.Text = year.ToString();
            viewHolder.TextViewDate.Text = month;
        }

        private static void BindEmptyItem(MonthCalendarViewHolder viewHolder)
        {
            viewHolder.ItemView.Visibility = ViewStates.Gone;
            viewHolder.TextViewDay.Text = string.Empty;
            viewHolder.TextViewDate.Text = string.Empty;
        }

        private void BindCalendarItem(MonthCalendarViewHolder viewHolder, RecyclerCalenderViewItem calendarItem)
        {
            try
            {
                string day = calendarItem.Date.ToString("ddd", new System.Globalization.CultureInfo(_configuration.CalendarLocale.ToString()!));
                viewHolder.TextViewDay.Text = day;

                int dateInt = int.Parse(calendarItem.Date.ToString("yyyyMMdd"));
                HandleEventForDate(viewHolder, dateInt);

                viewHolder.TextViewDate.Text = calendarItem.Date.ToString("dd");

                AttachClickListener(viewHolder, calendarItem, dateInt);
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("VerticalRecyclerCalendarAdapter", $"Error binding calendar item: {ex.Message}");
                // Handle the error appropriately
            }
        }

        private void HandleEventForDate(MonthCalendarViewHolder viewHolder, int dateInt)
        {
            if (_eventMap.TryGetValue(dateInt, out var @event))
            {
                viewHolder.ViewEvent.Visibility = ViewStates.Visible;
                viewHolder.ViewEvent.SetBackgroundColor(@event.Color);
            }
        }

        private void AttachClickListener(MonthCalendarViewHolder viewHolder, RecyclerCalenderViewItem calendarItem, int dateInt)
        {
            viewHolder.ItemView.Click += OnItemViewClick;

            void OnItemViewClick(object? sender, EventArgs e)
            {
                _eventMap.TryGetValue(dateInt, out var @event);
                _dateSelectListener.OnDateSelected(calendarItem.Date, @event);
            }
        }

        private sealed class MonthCalendarViewHolder : RecyclerView.ViewHolder
        {
            public TextView TextViewDay { get; }
            public TextView TextViewDate { get; }
            public View ViewEvent { get; }

            public MonthCalendarViewHolder(View itemView) : base(itemView ??
                throw new ArgumentNullException(nameof(itemView)))
            {
                TextViewDay = itemView.FindViewById<TextView>(Resource.Id.textCalenderItemVerticalDay)
                    ?? throw new InvalidOperationException("Required view textCalenderItemVerticalDay not found");
                TextViewDate = itemView.FindViewById<TextView>(Resource.Id.textCalenderItemVerticalDate)
                    ?? throw new InvalidOperationException("Required view textCalenderItemVerticalDate not found");
                ViewEvent = itemView.FindViewById(Resource.Id.viewCalenderItemVerticalEvent)
                    ?? throw new InvalidOperationException("Required view viewCalenderItemVerticalEvent not found");
            }
        }
    }
}