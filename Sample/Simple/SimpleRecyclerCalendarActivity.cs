using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Com.TejPratapSingh.RecyclerCalendar.Adapter;
using Com.TejPratapSingh.RecyclerCalendar.Model;
using Com.TejPratapSingh.RecyclerCalendar.Views;
using Google.Android.Material.Snackbar;
using Java.Util;

namespace Sample.Simple
{
    [Activity(Label = "@string/simple_recycler_calendar", Theme = "@style/AppTheme.NoActionBar")]
    public class SimpleRecyclerCalendarActivity : AppCompatActivity
    {
        private SimpleRecyclerCalendarView? _calendarView;
        private LinearLayout? _settingsContainer;
        private SimpleRecyclerCalendarConfiguration.SelectionMode? _selectionMode;
        private RecyclerCalendarConfiguration.CalendarViewType _calendarViewType =
            RecyclerCalendarConfiguration.CalendarViewType.Vertical;
        private DateTime _currentDate;
        private DateTime _endDate;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_simple_recycler_calendar);

            InitializeDates();
            InitializeViews();
            InitializeListeners();
            SetDefaultSelectionMode();
        }

        private void InitializeDates()
        {
            _currentDate = DateTime.Now;
            _endDate = _currentDate.AddMonths(3);
        }

        private void InitializeViews()
        {
            _calendarView = FindViewById<SimpleRecyclerCalendarView>(Resource.Id.calendarRecyclerView)
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
            var radioSelectNone = FindViewById<AppCompatRadioButton>(Resource.Id.radioSelectionNone)
                ?? throw new InvalidOperationException("None radio button not found");
            var radioSelectSingle = FindViewById<AppCompatRadioButton>(Resource.Id.radioSelectionSingle)
                ?? throw new InvalidOperationException("Single radio button not found");
            var radioSelectMultiple = FindViewById<AppCompatRadioButton>(Resource.Id.radioSelectionMultiple)
                ?? throw new InvalidOperationException("Multiple radio button not found");
            var radioSelectRange = FindViewById<AppCompatRadioButton>(Resource.Id.radioSelectionRange)
                ?? throw new InvalidOperationException("Range radio button not found");

            InitializeViewTypeListeners(radioViewTypeVertical, radioViewTypeHorizontal);
            InitializeSelectionModeListeners(radioSelectNone, radioSelectSingle, radioSelectMultiple, radioSelectRange);
        }

        private void InitializeViewTypeListeners(AppCompatRadioButton vertical, AppCompatRadioButton horizontal)
        {
            vertical.Click += (s, e) =>
            {
                _calendarViewType = RecyclerCalendarConfiguration.CalendarViewType.Vertical;
                RefreshCalendar();
            };

            horizontal.Click += (s, e) =>
            {
                _calendarViewType = RecyclerCalendarConfiguration.CalendarViewType.Horizontal;
                RefreshCalendar();
            };
        }

        private void InitializeSelectionModeListeners(
            AppCompatRadioButton none,
            AppCompatRadioButton single,
            AppCompatRadioButton multiple,
            AppCompatRadioButton range)
        {
            none.Click += (s, e) =>
            {
                _selectionMode = new SimpleRecyclerCalendarConfiguration.SelectionModeNone();
                RefreshCalendar();
            };

            single.Click += (s, e) =>
            {
                _selectionMode = new SimpleRecyclerCalendarConfiguration.SelectionModeSingle(_currentDate);
                RefreshCalendar();
            };

            multiple.Click += (s, e) =>
            {
                _selectionMode = new SimpleRecyclerCalendarConfiguration.SelectionModeMultiple(
                    new Dictionary<string, DateTime>()
                );
                RefreshCalendar();
            };

            range.Click += (s, e) =>
            {
                var rangeEndDate = _currentDate.AddDays(5);
                _selectionMode = new SimpleRecyclerCalendarConfiguration.SelectionModeRange(
                    _currentDate,
                    rangeEndDate
                );
                RefreshCalendar();
            };
        }

        private void SetDefaultSelectionMode()
        {
            var radioSelectNone = FindViewById<AppCompatRadioButton>(Resource.Id.radioSelectionNone);
            radioSelectNone?.PerformClick();
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

        private SimpleRecyclerCalendarConfiguration CreateCalendarConfiguration()
        {
            var configuration = new SimpleRecyclerCalendarConfiguration(
                calenderViewType: _calendarViewType,
                calendarLocale: Locale.Default,
                includeMonthHeader: true,
                selectionMode: _selectionMode ?? new SimpleRecyclerCalendarConfiguration.SelectionModeNone()
            );

            configuration.WeekStartOffset = RecyclerCalendarConfiguration.StartDayOfWeek.Monday;
            return configuration;
        }

        private void InitializeCalendarView(SimpleRecyclerCalendarConfiguration configuration)
        {
            _calendarView?.Initialise(
                _currentDate,
                _endDate,
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
            if (_calendarView == null) return;

            Snackbar.Make(
                _calendarView,
                message,
                Snackbar.LengthLong
            ).Show();
        }

        private void ShowError(string message)
        {
            if (_calendarView == null) return;

            Snackbar.Make(
                _calendarView,
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
            if (_settingsContainer != null)
            {
                var settingsButton = FindViewById<AppCompatImageButton>(Resource.Id.buttonSimpleSettings);
                settingsButton?.SetOnClickListener(null);
            }

            _calendarView = null;
            _settingsContainer = null;
            _selectionMode = null;
        }

        private class DateSelectedListener : Java.Lang.Object, SimpleRecyclerCalendarAdapter.IOnDateSelected
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