using Android.Content;
using Android.Util;
using Android.Views;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Com.TejPratapSingh.RecyclerCalendar.Model;
using Com.TejPratapSingh.RecyclerCalendar.Utilities;
using System;

namespace Com.TejPratapSingh.RecyclerCalendar.Adapter
{
    public class SimpleRecyclerCalendarAdapter : RecyclerCalendarBaseAdapter
    {
        public interface IOnDateSelected
        {
            void OnDateSelected(DateTime date);
        }

        public enum Position
        {
            None,   // Date is not part of Selection
            Single, // Single Selected Date
            Start,  // Selection Start
            Middle, // Selection Middle
            End     // Selection End
        }

        private readonly IOnDateSelected _dateSelectListener;
        private readonly SimpleRecyclerCalendarConfiguration _configuration;

        public SimpleRecyclerCalendarAdapter(
            DateTime startDate,
            DateTime endDate,
            SimpleRecyclerCalendarConfiguration configuration,
            IOnDateSelected dateSelectListener) : base(startDate, endDate, configuration)
        {
            _dateSelectListener = dateSelectListener;
            _configuration = configuration;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context)!.Inflate(Resource.Layout.item_simple_calendar, parent, false)!;
            return new SimpleCalendarViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position, RecyclerCalenderViewItem calendarItem)
        {
            var simpleViewHolder = (SimpleCalendarViewHolder)holder;
            var context = simpleViewHolder.ItemView.Context!;

            //Reset all views
            ResetViewHolder(simpleViewHolder);

            if (calendarItem.IsHeader)
            {
                BindHeaderItem(simpleViewHolder, calendarItem, context);
            }
            else if (calendarItem.IsEmpty)
            {
                BindEmptyItem(simpleViewHolder);
            }
            else
            {
                BindCalendarItem(simpleViewHolder, calendarItem, context);
            }
        }

        private void ResetViewHolder(SimpleCalendarViewHolder simpleViewHolder)
        {
            simpleViewHolder.ItemView.Visibility = ViewStates.Visible;
            simpleViewHolder.ItemView.Click -= ItemView_Click!;

            HighlightDate(simpleViewHolder, Position.None);

            var context = simpleViewHolder.ItemView.Context!;
            simpleViewHolder.TextViewDay.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.colorBlack)));
            simpleViewHolder.TextViewDate.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.colorBlack)));
            simpleViewHolder.TextViewDate.SetTextSize(ComplexUnitType.Sp, 14f);
        }

        private void BindHeaderItem(SimpleCalendarViewHolder simpleViewHolder, RecyclerCalenderViewItem calendarItem, Context context)
        {
            var selectedDate = calendarItem.Date;
            string month = selectedDate.ToString("MMMM", new System.Globalization.CultureInfo(_configuration.CalendarLocale.ToString()!));
            int year = selectedDate.Year;

            simpleViewHolder.TextViewDay.Text = year.ToString();
            simpleViewHolder.TextViewDate.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.grey_500)));
            simpleViewHolder.TextViewDate.SetTextSize(ComplexUnitType.Sp, 32f);
            simpleViewHolder.TextViewDate.Text = month;
        }

        private void BindEmptyItem(SimpleCalendarViewHolder simpleViewHolder)
        {
            simpleViewHolder.ItemView.Visibility = ViewStates.Gone;
            simpleViewHolder.TextViewDay.Text = "";
            simpleViewHolder.TextViewDate.Text = "";
        }

        private void BindCalendarItem(SimpleCalendarViewHolder simpleViewHolder, RecyclerCalenderViewItem calendarItem, Context context)
        {
            var selectedDate = calendarItem.Date;
            string currentDateString = selectedDate.ToString("yyyyMMdd", new System.Globalization.CultureInfo(_configuration.CalendarLocale.ToString()!));
            string currentWeekDay = selectedDate.ToString("ddd", new System.Globalization.CultureInfo(_configuration.CalendarLocale.ToString()!)).Substring(0, 2);

            simpleViewHolder.TextViewDay.Text = currentWeekDay;
            simpleViewHolder.TextViewDate.Text = CalendarUtils.DateStringFromFormat(_configuration.CalendarLocale, calendarItem.Date, CalendarUtils.DisplayDateFormat) ?? "";

            switch (_configuration.CurrentSelectionMode)
            {
                case SimpleRecyclerCalendarConfiguration.SelectionModeSingle selectionModeSingle:
                    BindSingleSelectionMode(simpleViewHolder, calendarItem, currentDateString, selectionModeSingle);
                    break;
                case SimpleRecyclerCalendarConfiguration.SelectionModeMultiple selectionModeMultiple:
                    BindMultipleSelectionMode(simpleViewHolder, calendarItem, currentDateString, selectionModeMultiple);
                    break;
                case SimpleRecyclerCalendarConfiguration.SelectionModeRange selectionModeRange:
                    BindRangeSelectionMode(simpleViewHolder, calendarItem, currentDateString, selectionModeRange);
                    break;
                default:
                    simpleViewHolder.ItemView.Click += (sender, e) => _dateSelectListener.OnDateSelected(calendarItem.Date);
                    break;
            }

            if (_configuration.ViewType == RecyclerCalendarConfiguration.CalendarViewType.Horizontal)
            {
                SetHorizontalItemWidth(simpleViewHolder, context);
            }
        }

        private void SetHorizontalItemWidth(SimpleCalendarViewHolder simpleViewHolder, Context context)
        {
            var displayMetrics = context.Resources!.DisplayMetrics;
            var screenWidth = displayMetrics!.WidthPixels;
            var itemWidth = screenWidth / 7;

            var layoutParams = simpleViewHolder.ItemView.LayoutParameters;
            layoutParams!.Width = itemWidth;
            simpleViewHolder.ItemView.LayoutParameters = layoutParams;
        }

        private void BindSingleSelectionMode(SimpleCalendarViewHolder simpleViewHolder, RecyclerCalenderViewItem calendarItem, string currentDateString, SimpleRecyclerCalendarConfiguration.SelectionModeSingle selectionModeSingle)
        {
            if (selectionModeSingle.SelectedDate != default)
            {
                string stringSelectedTimeFormat = CalendarUtils.DateStringFromFormat(_configuration.CalendarLocale, selectionModeSingle.SelectedDate, CalendarUtils.DbDateFormat) ?? "";

                if (currentDateString == stringSelectedTimeFormat)
                {
                    HighlightDate(simpleViewHolder, Position.Single);
                }

                simpleViewHolder.ItemView.Click += (sender, e) =>
                {
                    selectionModeSingle.SelectedDate = calendarItem.Date;
                    _dateSelectListener.OnDateSelected(calendarItem.Date);
                    NotifyDataSetChanged();
                };
            }
        }

        private void BindMultipleSelectionMode(SimpleCalendarViewHolder simpleViewHolder, RecyclerCalenderViewItem calendarItem, string currentDateString, SimpleRecyclerCalendarConfiguration.SelectionModeMultiple selectionModeMultiple)
        {
            var selectedDate = calendarItem.Date;
            string yesterdayDateString = selectedDate.AddDays(-1).ToString("yyyyMMdd", new System.Globalization.CultureInfo(_configuration.CalendarLocale.ToString()!));
            string tomorrowDateString = selectedDate.AddDays(1).ToString("yyyyMMdd", new System.Globalization.CultureInfo(_configuration.CalendarLocale.ToString()!));

            if (selectionModeMultiple.SelectionStartDateList.ContainsKey(currentDateString))
            {
                SetMultipleSelectionHighlight(simpleViewHolder, selectionModeMultiple, yesterdayDateString, tomorrowDateString);
            }

            simpleViewHolder.ItemView.Click += (sender, e) =>
            {
                if (!selectionModeMultiple.SelectionStartDateList.ContainsKey(currentDateString))
                {
                    selectionModeMultiple.SelectionStartDateList[currentDateString] = calendarItem.Date;
                }
                else
                {
                    selectionModeMultiple.SelectionStartDateList.Remove(currentDateString);
                }
                _dateSelectListener.OnDateSelected(calendarItem.Date);
                NotifyDataSetChanged();
            };
        }

        private void SetMultipleSelectionHighlight(SimpleCalendarViewHolder simpleViewHolder,
            SimpleRecyclerCalendarConfiguration.SelectionModeMultiple selectionModeMultiple,
            string yesterdayDateString, string tomorrowDateString)
        {
            if (selectionModeMultiple.SelectionStartDateList.ContainsKey(yesterdayDateString) &&
                selectionModeMultiple.SelectionStartDateList.ContainsKey(tomorrowDateString))
            {
                HighlightDate(simpleViewHolder, Position.Middle);
            }
            else if (selectionModeMultiple.SelectionStartDateList.ContainsKey(yesterdayDateString))
            {
                HighlightDate(simpleViewHolder, Position.End);
            }
            else if (selectionModeMultiple.SelectionStartDateList.ContainsKey(tomorrowDateString))
            {
                HighlightDate(simpleViewHolder, Position.Start);
            }
            else
            {
                HighlightDate(simpleViewHolder, Position.Single);
            }
        }

        private void BindRangeSelectionMode(SimpleCalendarViewHolder simpleViewHolder, RecyclerCalenderViewItem calendarItem, string currentDateString, SimpleRecyclerCalendarConfiguration.SelectionModeRange selectionModeRange)
        {
            DateTime startDate = selectionModeRange.SelectionStartDate;
            DateTime endDate = selectionModeRange.SelectionEndDate;

            int selectedDateInt = int.Parse(currentDateString);
            int startDateInt = int.Parse(CalendarUtils.DateStringFromFormat(_configuration.CalendarLocale, startDate, CalendarUtils.DbDateFormat) ?? int.MaxValue.ToString());
            int endDateInt = int.Parse(CalendarUtils.DateStringFromFormat(_configuration.CalendarLocale, endDate, CalendarUtils.DbDateFormat) ?? int.MinValue.ToString());

            if (selectedDateInt >= startDateInt && selectedDateInt <= endDateInt)
            {
                if (selectedDateInt == startDateInt)
                {
                    HighlightDate(simpleViewHolder, Position.Start);
                }
                else if (selectedDateInt == endDateInt)
                {
                    HighlightDate(simpleViewHolder, Position.End);
                }
                else
                {
                    HighlightDate(simpleViewHolder, Position.Middle);
                }
            }

            simpleViewHolder.ItemView.Click += (sender, e) =>
            {
                HandleRangeSelection(selectionModeRange, calendarItem.Date, selectedDateInt, startDateInt, endDateInt);
                _dateSelectListener.OnDateSelected(calendarItem.Date);
                NotifyDataSetChanged();
            };
        }

        private void HandleRangeSelection(SimpleRecyclerCalendarConfiguration.SelectionModeRange selectionModeRange,
            DateTime selectedDate, int selectedDateInt, int startDateInt, int endDateInt)
        {
            if (selectedDateInt < startDateInt)
            {
                selectionModeRange.SelectionStartDate = selectedDate;
            }
            else if (selectedDateInt > endDateInt)
            {
                selectionModeRange.SelectionEndDate = selectedDate;
            }
            else if (selectedDateInt > startDateInt && selectedDateInt < endDateInt)
            {
                var startDate = selectionModeRange.SelectionStartDate;
                var endDate = selectionModeRange.SelectionEndDate;

                if ((startDate - selectedDate).TotalMilliseconds > (selectedDate - endDate).TotalMilliseconds)
                {
                    selectionModeRange.SelectionEndDate = selectedDate;
                }
                else
                {
                    selectionModeRange.SelectionStartDate = selectedDate;
                }
            }
        }

        private void HighlightDate(SimpleCalendarViewHolder monthViewHolder, Position position)
        {
            var context = monthViewHolder.ItemView.Context!;

            switch (position)
            {
                case Position.Start:
                    monthViewHolder.LayoutContainer.SetBackgroundResource(Resource.Drawable.layout_round_corner_middle_filled);
                    monthViewHolder.LayoutContainerInner.SetBackgroundResource(Resource.Drawable.layout_round_corner_filled);
                    monthViewHolder.LayoutStartPadding.Visibility = ViewStates.Visible;
                    monthViewHolder.LayoutEndPadding.Visibility = ViewStates.Gone;
                    break;
                case Position.End:
                    monthViewHolder.LayoutContainer.SetBackgroundResource(Resource.Drawable.layout_round_corner_middle_filled);
                    monthViewHolder.LayoutContainerInner.SetBackgroundResource(Resource.Drawable.layout_round_corner_filled);
                    monthViewHolder.LayoutStartPadding.Visibility = ViewStates.Gone;
                    monthViewHolder.LayoutEndPadding.Visibility = ViewStates.Visible;
                    break;
                case Position.Middle:
                    monthViewHolder.LayoutContainer.SetBackgroundResource(Resource.Drawable.layout_round_corner_middle_filled);
                    monthViewHolder.LayoutContainerInner.Background = null;
                    monthViewHolder.LayoutStartPadding.Visibility = ViewStates.Gone;
                    monthViewHolder.LayoutEndPadding.Visibility = ViewStates.Gone;
                    break;
                case Position.Single:
                    monthViewHolder.LayoutContainer.Background = null;
                    monthViewHolder.LayoutContainerInner.SetBackgroundResource(Resource.Drawable.layout_round_corner_filled);
                    monthViewHolder.LayoutStartPadding.Visibility = ViewStates.Gone;
                    monthViewHolder.LayoutEndPadding.Visibility = ViewStates.Gone;
                    break;
                case Position.None:
                    monthViewHolder.LayoutContainer.Background = null;
                    monthViewHolder.LayoutContainerInner.Background = null;
                    monthViewHolder.LayoutStartPadding.Visibility = ViewStates.Gone;
                    monthViewHolder.LayoutEndPadding.Visibility = ViewStates.Gone;
                    break;
            }

            monthViewHolder.TextViewDay.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.colorWhite)));
            monthViewHolder.TextViewDate.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.colorWhite)));
        }

        private class SimpleCalendarViewHolder : RecyclerView.ViewHolder
        {
            public View LayoutStartPadding { get; }
            public View LayoutEndPadding { get; }
            public View LayoutContainer { get; }
            public LinearLayout LayoutContainerInner { get; }
            public TextView TextViewDay { get; }
            public TextView TextViewDate { get; }

            public SimpleCalendarViewHolder(View itemView) : base(itemView)
            {
                LayoutStartPadding = itemView.FindViewById(Resource.Id.layoutCalenderItemSimpleStartPadding)!;
                LayoutEndPadding = itemView.FindViewById(Resource.Id.layoutCalenderItemSimpleEndPadding)!;
                LayoutContainer = itemView.FindViewById(Resource.Id.layoutCalenderItemSimpleContainer)!;
                LayoutContainerInner = itemView.FindViewById<LinearLayout>(Resource.Id.layoutCalenderItemSimpleContainerInner)!;
                TextViewDay = itemView.FindViewById<TextView>(Resource.Id.textCalenderItemSimpleDay)!;
                TextViewDate = itemView.FindViewById<TextView>(Resource.Id.textCalenderItemSimpleDate)!;
            }
        }

        private void ItemView_Click(object sender, EventArgs e)
        {
            // Event handler implementation if needed
        }
    }
}