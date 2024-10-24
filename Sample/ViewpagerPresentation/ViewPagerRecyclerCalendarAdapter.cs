using Android.Views;
using AndroidX.RecyclerView.Widget;
using Com.TejPratapSingh.RecyclerCalendar.Adapter;
using Com.TejPratapSingh.RecyclerCalendar.Model;
using Sample.Model;

namespace Sample.ViewpagerPresentation
{
    public class ViewPagerRecyclerCalendarAdapter : RecyclerCalendarBaseAdapter
    {
        public interface IOnDateSelected
        {
            void OnDateSelected(DateTime date, SimpleEvent @event);
        }

        private readonly RecyclerCalendarConfiguration _configuration;
        private readonly Dictionary<int, SimpleEvent> _eventMap;
        private readonly IOnDateSelected _dateSelectListener;

        public ViewPagerRecyclerCalendarAdapter(
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
                .Inflate(Resource.Layout.item_calendar_view_pager, parent, false)!;
            return new MonthCalendarViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position, RecyclerCalenderViewItem calendarItem)
        {
            if (holder is not MonthCalendarViewHolder monthViewHolder)
                throw new ArgumentException("Incorrect ViewHolder type", nameof(holder));

            monthViewHolder.ItemView.Visibility = ViewStates.Visible;
            monthViewHolder.ProgressBar.Visibility = ViewStates.Gone;

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

        private void BindHeaderItem(MonthCalendarViewHolder viewHolder, RecyclerCalenderViewItem calendarItem)
        {
            var selectedDate = calendarItem.Date;
            string month = selectedDate.ToString("MMMM", new System.Globalization.CultureInfo(_configuration.CalendarLocale.ToString()!));
            int year = selectedDate.Year;

            viewHolder.TextViewDay.Text = year.ToString();
            viewHolder.TextViewDate.Text = month;
        }

        private void BindEmptyItem(MonthCalendarViewHolder viewHolder)
        {
            viewHolder.ItemView.Visibility = ViewStates.Gone;
            viewHolder.TextViewDay.Text = string.Empty;
            viewHolder.TextViewDate.Text = string.Empty;
        }

        private void BindCalendarItem(MonthCalendarViewHolder viewHolder, RecyclerCalenderViewItem calendarItem)
        {
            string day = calendarItem.Date.ToString("ddd", new System.Globalization.CultureInfo(_configuration.CalendarLocale.ToString()!));
            int dateInt = int.Parse(calendarItem.Date.ToString("yyyyMMdd"));

            if (_eventMap.TryGetValue(dateInt, out var @event))
            {
                viewHolder.ProgressBar.Visibility = ViewStates.Visible;
                viewHolder.ProgressBar.Progress = @event.Progress;
            }

            viewHolder.TextViewDay.Text = day;
            viewHolder.TextViewDate.Text = calendarItem.Date.ToString("dd");

            viewHolder.ItemView.Click -= OnItemViewClick;
            viewHolder.ItemView.Click += OnItemViewClick;

            void OnItemViewClick(object sender, EventArgs e)
            {
                _eventMap.TryGetValue(dateInt, out var clickedEvent);
                _dateSelectListener.OnDateSelected(calendarItem.Date, clickedEvent);
            }
        }

        private sealed class MonthCalendarViewHolder : RecyclerView.ViewHolder
        {
            public TextView TextViewDay { get; }
            public TextView TextViewDate { get; }
            public ProgressBar ProgressBar { get; }

            public MonthCalendarViewHolder(View itemView) : base(itemView)
            {
                TextViewDay = itemView.FindViewById<TextView>(Resource.Id.textCalenderItemViewPagerDay)
                    ?? throw new InvalidOperationException("Required view textCalenderItemViewPagerDay not found");
                TextViewDate = itemView.FindViewById<TextView>(Resource.Id.textCalenderItemViewPagerDate)
                    ?? throw new InvalidOperationException("Required view textCalenderItemViewPagerDate not found");
                ProgressBar = itemView.FindViewById<ProgressBar>(Resource.Id.progressBarCalenderItemViewPager)
                    ?? throw new InvalidOperationException("Required view progressBarCalenderItemViewPager not found");
            }
        }
    }
}