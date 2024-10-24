using Android.Graphics;
using Android.Views;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Com.TejPratapSingh.RecyclerCalendar.Model;
using Google.Android.Material.Dialog;
using Google.Android.Material.Snackbar;
using Java.Util;
using Sample.Model;

namespace Sample.ViewpagerPresentation
{
    public class PlaceholderFragment : AndroidX.Fragment.App.Fragment
    {
        private const string ARG_SECTION_NUMBER = "section_number";
        private const int EVENT_INTERVAL_DAYS = 3;

        private readonly Dictionary<int, SimpleEvent> _eventMap = new();
        private RecyclerView? _calendarRecyclerView;
        private RecyclerView.Adapter? _calendarAdapter;
        private DateTime _currentDate;

        public static PlaceholderFragment NewInstance(int sectionNumber)
        {
            var fragment = new PlaceholderFragment();
            var args = new Bundle();
            args.PutInt(ARG_SECTION_NUMBER, sectionNumber);
            fragment.Arguments = args;
            return fragment;
        }

        public override View OnCreateView(
            LayoutInflater inflater,
            ViewGroup? container,
            Bundle? savedInstanceState)
        {
            var root = inflater.Inflate(_Microsoft.Android.Resource.Designer.ResourceConstant.Layout.fragment_view_pager_calendar, container, false)
                ?? throw new InvalidOperationException("Failed to inflate fragment layout");

            var sectionNumber = Arguments?.GetInt(ARG_SECTION_NUMBER)
                ?? throw new InvalidOperationException("Section number not provided");

            InitializeViews(root);
            InitializeDates(sectionNumber);
            InitializeEvents();
            InitializeCalendar();

            return root;
        }

        private void InitializeViews(View root)
        {
            _calendarRecyclerView = root.FindViewById<RecyclerView>(_Microsoft.Android.Resource.Designer.ResourceConstant.Id.calendarRecyclerView)
                ?? throw new InvalidOperationException("Calendar RecyclerView not found");
        }

        private void InitializeDates(int sectionNumber)
        {
            _currentDate = DateTime.Now.AddMonths(sectionNumber);
        }

        private void InitializeEvents()
        {
            if (Context == null) return;

            var random = new Java.Util.Random();
            var accentColor = new Color(ContextCompat.GetColor(Context, _Microsoft.Android.Resource.Designer.ResourceConstant.Color.colorAccent));

            for (int i = 0; i <= 30; i += EVENT_INTERVAL_DAYS)
            {
                var eventDate = _currentDate.AddDays(i * EVENT_INTERVAL_DAYS);
                var eventDateInt = int.Parse(eventDate.ToString("yyyyMMdd"));

                _eventMap[eventDateInt] = new SimpleEvent(
                    date: eventDate,
                    title: $"Event #{i}",
                    color: accentColor,
                    progress: i * EVENT_INTERVAL_DAYS
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
                includeMonthHeader: false
            );

            configuration.WeekStartOffset = RecyclerCalendarConfiguration.StartDayOfWeek.Monday;
            return configuration;
        }

        private void SetupCalendarAdapter(RecyclerCalendarConfiguration configuration)
        {
            _calendarAdapter = new ViewPagerRecyclerCalendarAdapter(
                startDate: _currentDate,
                endDate: _currentDate,
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
                new LinearLayoutManager(Context, LinearLayoutManager.Vertical, false));

            // Set adapter
            _calendarRecyclerView.SetAdapter(_calendarAdapter);

            // Optional: Enable item animations
            _calendarRecyclerView.SetItemAnimator(new DefaultItemAnimator());
        }

        private void OnDateSelected(DateTime date, SimpleEvent? @event)
        {
            if (Activity == null || @event == null) return;

            var selectedDate = date.ToString("D", System.Globalization.CultureInfo.CurrentCulture);
            ShowEventDialog(selectedDate, @event);
        }

        private void ShowEventDialog(string selectedDate, SimpleEvent @event)
        {
            if (Activity == null) return;

            var materialAlertBuilder = new MaterialAlertDialogBuilder(Activity)
                   .SetTitle("Event Clicked")
                .SetMessage(GetEventMessage(selectedDate, @event))
                .SetPositiveButton("Ok", (_, _) => { })
                .SetIcon(_Microsoft.Android.Resource.Designer.ResourceConstant.Drawable.m3_tabs_rounded_line_indicator);

            materialAlertBuilder.Create()?.Show();
        }

        private static string GetEventMessage(string selectedDate, SimpleEvent @event)
        {
            return $"Date: {selectedDate}\n\nEvent: {@event.Title}\n\nProgress: {@event.Progress}%";
        }

        private void ShowError(string message)
        {
            if (Context == null || Activity == null || _calendarRecyclerView == null) return;

            Snackbar
                 .Make(_calendarRecyclerView, message, Snackbar.LengthLong)
                 .SetAction("OK", (_) => { })
                 .Show();
        }

        public override void OnDestroyView()
        {
            CleanupResources();
            base.OnDestroyView();
        }

        private void CleanupResources()
        {
            _eventMap.Clear();
            _calendarAdapter = null;
            _calendarRecyclerView = null;
        }

        private class DateSelectedListener : Java.Lang.Object, ViewPagerRecyclerCalendarAdapter.IOnDateSelected
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