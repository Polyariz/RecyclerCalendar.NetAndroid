using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Com.TejPratapSingh.RecyclerCalendar.Adapter;
using Com.TejPratapSingh.RecyclerCalendar.Model;
using Com.TejPratapSingh.RecyclerCalendar.Views;
using Java.Util;

namespace Sample.Simple
{
    [Activity(Label = "@string/infinite_recycler_calendar", Theme = "@style/AppTheme.NoActionBar")]
    public class InfiniteRecyclerCalendarActivity : AppCompatActivity
    {
        private InfiniteRecyclerCalendarView? _calendarView;
        private LinearLayout? _settingsContainer;
        private InfiniteRecyclerCalendarConfiguration.SelectionMode? _selectionMode;
        private RecyclerCalendarConfiguration.CalendarViewType _calendarViewType =
            RecyclerCalendarConfiguration.CalendarViewType.Vertical;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_infinite_recycler_calendar);

            InitializeViews();
            InitializeListeners();
            InitializeCalendar();
        }

        private void InitializeViews()
        {
            _calendarView = FindViewById<InfiniteRecyclerCalendarView>(Resource.Id.calendarRecyclerView)
                ?? throw new InvalidOperationException("Calendar view not found");

            _settingsContainer = FindViewById<LinearLayout>(Resource.Id.layoutSettingContainer)
                ?? throw new InvalidOperationException("Settings container not found");

            var settingsButton = FindViewById<AppCompatImageButton>(Resource.Id.buttonSimpleSettings)
                ?? throw new InvalidOperationException("Settings button not found");

            settingsButton.Click += OnSettingsButtonClick;
        }

        private void InitializeListeners()
        {
            var radioViewTypeVertical = FindViewById<AppCompatRadioButton>(Resource.Id.radioViewTypeVertical)
                ?? throw new InvalidOperationException("Vertical radio button not found");
            var radioViewTypeHorizontal = FindViewById<AppCompatRadioButton>(Resource.Id.radioViewTypeHorizontal)
                ?? throw new InvalidOperationException("Horizontal radio button not found");

            radioViewTypeVertical.Click += (s, e) =>
            {
                _calendarViewType = RecyclerCalendarConfiguration.CalendarViewType.Vertical;
                RefreshCalendar();
            };

            radioViewTypeHorizontal.Click += (s, e) =>
            {
                _calendarViewType = RecyclerCalendarConfiguration.CalendarViewType.Horizontal;
                RefreshCalendar();
            };
        }

        private void InitializeCalendar()
        {
            _selectionMode = new InfiniteRecyclerCalendarConfiguration.SelectionModeNone();
            RefreshCalendar();
        }

        private void OnSettingsButtonClick(object? sender, EventArgs e)
        {
            if (_settingsContainer == null) return;

            _settingsContainer.Visibility = _settingsContainer.Visibility switch
            {
                ViewStates.Visible => ViewStates.Gone,
                _ => ViewStates.Visible
            };
        }

        private void RefreshCalendar()
        {
            if (_calendarView == null || _selectionMode == null) return;

            try
            {
                var configuration = CreateCalendarConfiguration();
                InitializeCalendarView(configuration);
            }
            catch (Exception ex)
            {
                ShowError($"Failed to refresh calendar: {ex.Message}");
            }
        }

        private InfiniteRecyclerCalendarConfiguration CreateCalendarConfiguration()
        {
            var configuration = new InfiniteRecyclerCalendarConfiguration(
                viewType: _calendarViewType,
                calendarLocale: Locale.Default,
                includeMonthHeader: true,
                selectionMode: _selectionMode ?? new InfiniteRecyclerCalendarConfiguration.SelectionModeNone()
            );

            configuration.WeekStartOffset = RecyclerCalendarConfiguration.StartDayOfWeek.Monday;
            return configuration;
        }

        private void InitializeCalendarView(InfiniteRecyclerCalendarConfiguration configuration)
        {
            _calendarView?.Initialize(
                configuration,
                new DateSelectedListener(OnDateSelected)
            );
        }

        private void OnDateSelected(DateTime date)
        {
            var formattedDate = date.ToString("G", System.Globalization.CultureInfo.CurrentCulture);
            ShowMessage($"Date Selected: {formattedDate}");
        }

        private void ShowMessage(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long)?.Show();
        }

        private void ShowError(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long)?.Show();
        }

        protected override void OnDestroy()
        {
            CleanupResources();
            base.OnDestroy();
        }

        private void CleanupResources()
        {
            if (_settingsContainer != null)
            {
                var settingsButton = FindViewById<AppCompatImageButton>(Resource.Id.buttonSimpleSettings);
                settingsButton?.SetOnClickListener(null);
            }

            _calendarView = null;
            _settingsContainer = null;
            _selectionMode = null;
        }

        private class DateSelectedListener : Java.Lang.Object, InfiniteRecyclerCalendarAdapter.OnDateSelected
        {
            private readonly Action<DateTime> _onDateSelected;

            public DateSelectedListener(Action<DateTime> onDateSelected)
            {
                _onDateSelected = onDateSelected ?? throw new ArgumentNullException(nameof(onDateSelected));
            }

            public void OnDateSelected(DateTime date)
            {
                _onDateSelected(date);
            }
        }
    }
}