using Android.Views;

namespace Com.TejPratapSingh.RecyclerCalendar.Model
{
    public class OnClickListenerEvent : Java.Lang.Object, View.IOnClickListener
    {
        private readonly Action _onClickAction;

        public OnClickListenerEvent(Action onClickAction)
        {
            _onClickAction = onClickAction;
        }

        public void OnClick(View? v)
        {
            _onClickAction();
        }
    }
}
