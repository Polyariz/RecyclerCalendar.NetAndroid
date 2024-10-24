using Android.Graphics;

namespace Sample.Model
{
    public class SimpleEvent
    {
        public DateTime Date { get; }
        public string Title { get; }
        public Color Color { get; }
        public int Progress { get; }

        public SimpleEvent(DateTime date, string title, Color color, int progress = 0)
        {
            Date = date;
            Title = title;
            Color = color;
            Progress = progress;
        }
    }
}
