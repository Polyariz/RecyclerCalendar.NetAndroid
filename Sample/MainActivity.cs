using Android.Content;
using Sample.Horizontal;
using Sample.Simple;
using Sample.Vertical;
using Sample.ViewpagerPresentation;

namespace Sample
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            var layoutWeekCalendar = FindViewById<LinearLayout>(Resource.Id.layoutWeekCalendarActivity);
            layoutWeekCalendar.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(HorizontalCalendarActivity));
                StartActivity(intent);
            };

            var layoutMonthCalendar = FindViewById<LinearLayout>(Resource.Id.layoutMonthCalendarActivity);
            layoutMonthCalendar.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(VerticalCalendarActivity));
                StartActivity(intent);
            };

            var layoutPageCalendar = FindViewById<LinearLayout>(Resource.Id.layoutPageCalendarActivity);
            layoutPageCalendar.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(ViewPagerCalendarActivity));
                StartActivity(intent);
            };

            var layoutSimpleCalendar = FindViewById<LinearLayout>(Resource.Id.layoutSimpleCalendarActivity);
            layoutSimpleCalendar.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(SimpleRecyclerCalendarActivity));
                StartActivity(intent);
            };

            var layoutInfiniteCalendar = FindViewById<LinearLayout>(Resource.Id.layoutInfiniteCalendarActivity);
            layoutInfiniteCalendar.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(InfiniteRecyclerCalendarActivity));
                StartActivity(intent);
            };
        }
    }
}