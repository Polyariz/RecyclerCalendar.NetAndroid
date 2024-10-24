using Android.Graphics;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Com.TejPratapSingh.RecyclerCalendar.Model;
using Google.Android.Material.Dialog;
using Google.Android.Material.Snackbar;
using Java.Util;
using Sample.Model;

namespace Sample.Vertical
{
    [Activity(Label = "@string/vertical_calendar", Theme = "@style/AppTheme.NoActionBar")]
    public class VerticalCalendarActivity : AppCompatActivity
    {
        private const int EVENT_INTERVAL_DAYS = 3;
        private const int MONTHS_TO_ADD = 12;

        private readonly Dictionary<int, SimpleEvent> _eventMap = new();
        private RecyclerView? _calendarRecyclerView;
        private DateTime _currentDate;
        private DateTime _endDate;
        private RecyclerView.Adapter? _calendarAdapter;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_vertical_calendar);

            InitializeDates();
            InitializeViews();
            InitializeEvents();
            InitializeCalendar();
        }

        private void InitializeDates()
        {
            _currentDate = DateTime.Now;
            _endDate = _currentDate.AddMonths(MONTHS_TO_ADD);
        }

        private void InitializeViews()
        {
            _calendarRecyclerView = FindViewById<RecyclerView>(Resource.Id.calendarRecyclerView)
                ?? throw new InvalidOperationException("Calendar RecyclerView not found");
        }

        private void InitializeEvents()
        {
            var accentColor = new Color(ContextCompat.GetColor(this, Resource.Color.colorAccent));

            for (int i = 0; i <= 30; i += EVENT_INTERVAL_DAYS)
            {
                var eventDate = _currentDate.AddDays(i * EVENT_INTERVAL_DAYS);
                var eventDateInt = int.Parse(eventDate.ToString("yyyyMMdd"));

                _eventMap[eventDateInt] = new SimpleEvent(
                    date: eventDate,
                    title: $"Event #{i}",
                    color: accentColor
                );
            }
        }

        private void InitializeCalendar()
        {
            try
            {
                var configuration = CreateCalendarConfiguration();
                SetupCalendarAdapter(configuration);
                SetupRecyclerView();
            }
            catch (Exception ex)
            {
                ShowError($"Failed to initialize calendar: {ex.Message}");
            }
        }

        private RecyclerCalendarConfiguration CreateCalendarConfiguration()
        {
            var configuration = new RecyclerCalendarConfiguration(
                RecyclerCalendarConfiguration.CalendarViewType.Vertical,
                Locale.Default,
                includeMonthHeader: true
            );

            configuration.WeekStartOffset = RecyclerCalendarConfiguration.StartDayOfWeek.Monday;
            return configuration;
        }

        private void SetupCalendarAdapter(RecyclerCalendarConfiguration configuration)
        {
            _calendarAdapter = new VerticalRecyclerCalendarAdapter(
                startDate: _currentDate,
                endDate: _endDate,
                configuration: configuration,
                eventMap: _eventMap,
                dateSelectListener: new DateSelectedListener(OnDateSelected)
            );
        }

        private void SetupRecyclerView()
        {
            if (_calendarRecyclerView == null || _calendarAdapter == null) return;

            // Set layout manager
            _calendarRecyclerView.SetLayoutManager(
                new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));

            // Set adapter
            _calendarRecyclerView.SetAdapter(_calendarAdapter);

            // Optional: Enable item animations
            _calendarRecyclerView.SetItemAnimator(new DefaultItemAnimator());

            // Optional: Add item decoration if needed
            // _calendarRecyclerView.AddItemDecoration(
            //     new DividerItemDecoration(this, DividerItemDecoration.Vertical));
        }

        private void OnDateSelected(DateTime date, SimpleEvent? @event)
        {
            var selectedDate = date.ToString("D", System.Globalization.CultureInfo.CurrentCulture);

            if (@event != null)
            {
                ShowEventDialog(selectedDate, @event);
            }
            else
            {
                ShowMessage($"Selected date: {selectedDate}");
            }
        }

        private void ShowEventDialog(string selectedDate, SimpleEvent @event)
        {
            var materialAlertBuilder = new MaterialAlertDialogBuilder(this)
                .SetTitle("Event Clicked")
                .SetMessage(GetEventMessage(selectedDate, @event))
                .SetPositiveButton("Ok", (_, _) => { })
                .SetIcon(Resource.Drawable.m3_tabs_rounded_line_indicator);

            materialAlertBuilder.Create()?.Show();
        }

        private static string GetEventMessage(string selectedDate, SimpleEvent @event)
        {
            return $"Date: {selectedDate}\n\nEvent: {@event.Title}";
        }

        private void ShowMessage(string message)
        {
            if (_calendarRecyclerView == null) return;

            Snackbar.Make(
                _calendarRecyclerView,
                message,
                Snackbar.LengthLong
            ).Show();
        }

        private void ShowError(string message)
        {
            if (_calendarRecyclerView == null) return;

            Snackbar.Make(
                _calendarRecyclerView,
                message,
                Snackbar.LengthLong
            )
            .SetAction("OK", (_) => { })
            .Show();
        }

        protected override void OnDestroy()
        {
            CleanupResources();
            base.OnDestroy();
        }

        private void CleanupResources()
        {
            _eventMap.Clear();
            _calendarAdapter = null;
            _calendarRecyclerView = null;
        }

        private class DateSelectedListener : Java.Lang.Object, VerticalRecyclerCalendarAdapter.IOnDateSelected
        {
            private readonly Action<DateTime, SimpleEvent?> _onDateSelected;

            public DateSelectedListener(Action<DateTime, SimpleEvent?> onDateSelected)
            {
                _onDateSelected = onDateSelected ?? throw new ArgumentNullException(nameof(onDateSelected));
            }

            public void OnDateSelected(DateTime date, SimpleEvent? @event)
            {
                _onDateSelected(date, @event);
            }
        }
    }
}