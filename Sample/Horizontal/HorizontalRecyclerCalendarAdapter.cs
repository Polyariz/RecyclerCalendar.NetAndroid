using Android.OS;
using Android.Views;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Com.TejPratapSingh.RecyclerCalendar.Adapter;
using Com.TejPratapSingh.RecyclerCalendar.Model;

namespace Sample.Horizontal
{
    public class HorizontalRecyclerCalendarAdapter : RecyclerCalendarBaseAdapter
    {
        public interface IOnDateSelected
        {
            void OnDateSelected(DateTime date);
        }

        private readonly RecyclerCalendarConfiguration _configuration;
        private DateTime _selectedDate;
        private readonly IOnDateSelected _dateSelectListener;

        public HorizontalRecyclerCalendarAdapter(
            DateTime startDate,
            DateTime endDate,
            RecyclerCalendarConfiguration configuration,
            DateTime selectedDate,
            IOnDateSelected dateSelectListener) : base(startDate, endDate, configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _selectedDate = selectedDate;
            _dateSelectListener = dateSelectListener ?? throw new ArgumentNullException(nameof(dateSelectListener));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context)!
                .Inflate(Resource.Layout.item_calendar_horizontal, parent, false)!;
            return new MonthCalendarViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position, RecyclerCalenderViewItem calendarItem)
        {
            if (holder is not MonthCalendarViewHolder monthViewHolder)
                throw new ArgumentException("Incorrect ViewHolder type", nameof(holder));

            var context = monthViewHolder.ItemView.Context!;
            InitializeViewHolder(monthViewHolder, context);

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
                BindCalendarItem(monthViewHolder, calendarItem, context);
            }
        }

        private void InitializeViewHolder(MonthCalendarViewHolder viewHolder, Android.Content.Context context)
        {
            viewHolder.ItemView.Visibility = ViewStates.Visible;
            viewHolder.ItemView.Background = null;
            viewHolder.ItemView.SetOnClickListener(null);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean) 
                viewHolder.ItemView.Background = null; 
            else 
                viewHolder.ItemView.SetBackgroundDrawable(null); 

            SetTextColors(viewHolder, context, Resource.Color.colorBlack);
        }

        private void BindHeaderItem(MonthCalendarViewHolder viewHolder, RecyclerCalenderViewItem calendarItem)
        {
            string month = calendarItem.Date.ToString("MMMM", new System.Globalization.CultureInfo(_configuration.CalendarLocale.ToString()!));
            int year = calendarItem.Date.Year;

            viewHolder.TextViewDay.Text = year.ToString();
            viewHolder.TextViewDate.Text = month;
            viewHolder.ItemView.SetOnClickListener(null);
        }

        private void BindEmptyItem(MonthCalendarViewHolder viewHolder)
        {
            viewHolder.ItemView.Visibility = ViewStates.Gone;
            viewHolder.TextViewDay.Text = string.Empty;
            viewHolder.TextViewDate.Text = string.Empty;
        }

        private void BindCalendarItem(MonthCalendarViewHolder viewHolder, RecyclerCalenderViewItem calendarItem, Android.Content.Context context)
        {
            string currentDateString = calendarItem.Date.ToString("yyyyMMdd");
            string selectedDateString = _selectedDate.ToString("yyyyMMdd");

            if (currentDateString == selectedDateString)
            {
                ApplySelectedStyle(viewHolder, context);
            }

            string day = calendarItem.Date.ToString("ddd", new System.Globalization.CultureInfo(_configuration.CalendarLocale.ToString()!));
            viewHolder.TextViewDay.Text = day;
            viewHolder.TextViewDate.Text = calendarItem.Date.ToString("dd");

            viewHolder.ItemView.SetOnClickListener(new ClickListener(() =>
            {
                _selectedDate = calendarItem.Date;
                _dateSelectListener.OnDateSelected(calendarItem.Date);
                NotifyDataSetChanged();
            })); 
        }

        private void ApplySelectedStyle(MonthCalendarViewHolder viewHolder, Android.Content.Context context)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
            {
                viewHolder.ItemView.Background = ContextCompat.GetDrawable(context, Resource.Drawable.layout_round_corner_filled);
            }
            else
            {
                viewHolder.ItemView.SetBackgroundResource(Resource.Drawable.layout_round_corner_filled);
            }
            SetTextColors(viewHolder, context, Resource.Color.colorWhite);
        }

        private static void SetTextColors(MonthCalendarViewHolder viewHolder, Android.Content.Context context, int colorResourceId)
        {
            var color = new Android.Graphics.Color(ContextCompat.GetColor(context, colorResourceId));
            viewHolder.TextViewDay.SetTextColor(color);
            viewHolder.TextViewDate.SetTextColor(color);
        }

        private sealed class MonthCalendarViewHolder : RecyclerView.ViewHolder
        {
            public TextView TextViewDay { get; }
            public TextView TextViewDate { get; }

            public MonthCalendarViewHolder(View itemView) : base(itemView)
            {
                TextViewDay = itemView.FindViewById<TextView>(Resource.Id.textCalenderItemHorizontalDay)
                    ?? throw new InvalidOperationException("Required view textCalenderItemHorizontalDay not found");
                TextViewDate = itemView.FindViewById<TextView>(Resource.Id.textCalenderItemHorizontalDate)
                    ?? throw new InvalidOperationException("Required view textCalenderItemHorizontalDate not found");
            }
        } 
        private class ClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private readonly Action _onClickAction;

            public ClickListener(Action onClickAction)
            {
                _onClickAction = onClickAction;
            }

            public void OnClick(View v)
            {
                _onClickAction();
            }
        }
    }
}