namespace Com.TejPratapSingh.RecyclerCalendar.Model
{
    /// <summary>
    /// Represents a calendar item in the recycler view
    /// </summary>
    public class RecyclerCalenderViewItem
    {
        // Date of calendar item
        public DateTime Date { get; set; }

        // Span size of cell for grid view
        // Month Header has span size of 7 (full week)
        // For offset for new month, we use empty space which can have span from 0-6
        public int SpanSize { get; set; }

        // For offset of new month
        public bool IsEmpty { get; set; }

        // Header is simply a Month name cell
        public bool IsHeader { get; set; }

        public RecyclerCalenderViewItem(DateTime date, int spanSize, bool isEmpty, bool isHeader)
        {
            Date = date;
            SpanSize = spanSize;
            IsEmpty = isEmpty;
            IsHeader = isHeader;
        }

        public override string ToString() =>
            $"date: {Date:yyyy-MM-dd HH:mm:ss}, spanSize: {SpanSize}, isEmpty: {IsEmpty}, isHeader: {IsHeader}";
    }
}
