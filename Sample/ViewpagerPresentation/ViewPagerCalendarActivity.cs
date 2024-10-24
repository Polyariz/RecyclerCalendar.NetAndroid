
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.Tabs;
using Sample;
using Sample.ViewpagerPresentation;

[Activity(Label = "@string/viewpager_calendar", Theme = "@style/AppTheme.NoActionBar")]
public class ViewPagerCalendarActivity : AppCompatActivity
{
    private ViewPager2? _viewPager;
    private TabLayout? _tabLayout;
    private SectionsPagerAdapter? _sectionsPagerAdapter;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_view_pager_calendar);

        InitializeViews();
        SetupViewPager();
        ConfigureRtlSupport();
    }

    private void InitializeViews()
    {
        _viewPager = FindViewById<ViewPager2>(Resource.Id.view_pager)
            ?? throw new InvalidOperationException("ViewPager not found");

        _tabLayout = FindViewById<TabLayout>(Resource.Id.tabs)
            ?? throw new InvalidOperationException("TabLayout not found");
    }

    private void SetupViewPager()
    {
        if (_viewPager == null || _tabLayout == null) return;

        _sectionsPagerAdapter = new SectionsPagerAdapter(this);
        _viewPager.Adapter = _sectionsPagerAdapter;

        // Connect TabLayout with ViewPager2
        new TabLayoutMediator(
        _tabLayout,
        _viewPager,
            new TabConfigurationStrategy(_sectionsPagerAdapter)
        ).Attach();
    }

    private void ConfigureRtlSupport()
    {
        if (_tabLayout == null || Build.VERSION.SdkInt < BuildVersionCodes.JellyBeanMr1) return;

        var config = Resources?.Configuration;
        if (config?.LayoutDirection == LayoutDirection.Rtl)
        {
            _tabLayout.LayoutDirection = LayoutDirection.Ltr;
        }
    }

    protected override void OnDestroy()
    {
        CleanupResources();
        base.OnDestroy();
    }

    private void CleanupResources()
    {
        _viewPager = null;
        _tabLayout = null;
        _sectionsPagerAdapter = null;
    }

    public class TabConfigurationStrategy : Java.Lang.Object, TabLayoutMediator.ITabConfigurationStrategy
    {
        private readonly SectionsPagerAdapter _adapter;

        public TabConfigurationStrategy(SectionsPagerAdapter adapter)
        {
            _adapter = adapter;
        }

        public void OnConfigureTab(TabLayout.Tab tab, int position)
        {
            tab.SetText(_adapter.GetPageTitle(position)?.ToString());
        }
    }
}