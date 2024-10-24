using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using Java.Lang;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Sample.ViewpagerPresentation
{
    public class SectionsPagerAdapter : FragmentStateAdapter
    {
        private const int TOTAL_PAGES = 24;
        public SectionsPagerAdapter(FragmentActivity activity) : base(activity)
        {
        }

        public override Fragment CreateFragment(int position)
        {
            return PlaceholderFragment.NewInstance(position);
        }

        public override int ItemCount => TOTAL_PAGES;

        public ICharSequence? GetPageTitle(int position)
        {
            try
            {
                var date = DateTime.Now.AddMonths(position);
                var month = date.ToString("MMMM", System.Globalization.CultureInfo.CurrentCulture);
                var year = date.Year;
                return new Java.Lang.String($"{month} / {year}");
            }
            catch (System.Exception ex)
            {
                Android.Util.Log.Error("SectionsPagerAdapter", $"Error getting page title: {ex.Message}");
                return new Java.Lang.String($"Page {position + 1}");
            }
        }
    }
}