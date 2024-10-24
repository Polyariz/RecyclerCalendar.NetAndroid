using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Com.TejPratapSingh.RecyclerCalendar.Model;
using Google.Android.Material.TextView;
using Java.Util;

namespace Sample.Horizontal
{
    [Activity(Label = "@string/horizontal_calendar", Theme = "@style/AppTheme.NoActionBar")]
    public class HorizontalCalendarActivity : AppCompatActivity
    {
        private RecyclerView? _calendarRecyclerView;
        private TextView? _selectedDateTextView;
        private RecyclerView.Adapter? _calendarAdapter;
        private PagerSnapHelper? _snapHelper;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_horizontal_calendar);

            InitializeViews();
            InitializeCalendar();
        }

        protected override void OnDestroy()
        {
            CleanupResources();
            base.OnDestroy();
        }

        private void InitializeViews()
        {
            _calendarRecyclerView = FindViewById<RecyclerView>(Resource.Id.calendarRecyclerView)
                ?? throw new InvalidOperationException("Calendar RecyclerView not found");

            _selectedDateTextView = FindViewById<TextView>(Resource.Id.textViewSelectedDate)
                ?? throw new InvalidOperationException("Selected date TextView not found");
        }

        private void InitializeCalendar()
        {
            var currentDate = DateTime.Now;
            var configuration = CreateCalendarConfiguration();

            UpdateSelectedDateDisplay(currentDate);
            SetupCalendarAdapter(currentDate, configuration);
            SetupRecyclerView();
        }

        private RecyclerCalendarConfiguration CreateCalendarConfiguration()
        {
            var configuration = new RecyclerCalendarConfiguration(
                viewType: RecyclerCalendarConfiguration.CalendarViewType.Horizontal,
                calendarLocale: Locale.Default,
                includeMonthHeader: true
            );

            configuration.WeekStartOffset = RecyclerCalendarConfiguration.StartDayOfWeek.Monday;
            return configuration;
        }

        private void UpdateSelectedDateDisplay(DateTime date)
        {
            if (_selectedDateTextView == null) return;

            _selectedDateTextView.Text = date.ToString("D",
                System.Globalization.CultureInfo.CurrentCulture);
        }

        private void SetupCalendarAdapter(DateTime currentDate, RecyclerCalendarConfiguration configuration)
        {
            var startDate = currentDate;
            var endDate = currentDate.AddMonths(3);

            _calendarAdapter = new HorizontalRecyclerCalendarAdapter(
                startDate: startDate,
                endDate: endDate,
                configuration: configuration,
                selectedDate: currentDate,
                dateSelectListener: new DateSelectedListener(OnDateSelected)
            );
        }

        private void SetupRecyclerView()
        {
            if (_calendarRecyclerView == null || _calendarAdapter == null) return;

            // Set layout manager
            _calendarRecyclerView.SetLayoutManager(
                new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));

            // Set adapter
            _calendarRecyclerView.SetAdapter(_calendarAdapter);

            // Setup snap helper
            _snapHelper = new PagerSnapHelper();
            _calendarRecyclerView.SetOnFlingListener(null); // Clear any existing fling listener
            _snapHelper.AttachToRecyclerView(_calendarRecyclerView);

            // Optional: Set item decoration if needed
            //_calendarRecyclerView.AddItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.Horizontal));

            // Optional: Enable item animations
            _calendarRecyclerView.SetItemAnimator(new DefaultItemAnimator());
        }

        private void OnDateSelected(DateTime date)
        {
            UpdateSelectedDateDisplay(date);
        }

        private void CleanupResources()
        {
            if (_calendarRecyclerView != null && _snapHelper != null)
            {
                _snapHelper.AttachToRecyclerView(null);
                _snapHelper = null;
            }

            _calendarAdapter = null;
            _calendarRecyclerView = null;
            _selectedDateTextView = null;
        }

        private class DateSelectedListener : Java.Lang.Object, HorizontalRecyclerCalendarAdapter.IOnDateSelected
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