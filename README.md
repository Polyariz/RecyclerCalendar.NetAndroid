

# RecyclerCalendar.NetAndroid (Xamarin)
  
A DIY calendar generator library for Xamarin .Net Android. This is a C# port of RecyclerCalendarAndroid Kotlin library [RecyclerCalendarAndroid](https://github.com/tejpratap46/RecyclerCalendarAndroid)

### Here are sample calenders you can create with this library :

| Week Calendar | Month With Events | Month With Swipe Pages |
| -- | -- | -- |
| Code At: [horizontal](https://github.com/tejpratap46/RecyclerCalendarAndroid/tree/master/app/src/main/java/com/tejpratapsingh/recyclercalendaractivity/horizontal) | Code At: [vertical](https://github.com/tejpratap46/RecyclerCalendarAndroid/tree/master/app/src/main/java/com/tejpratapsingh/recyclercalendaractivity/vertical) | Code At: [viewpager](https://github.com/tejpratap46/RecyclerCalendarAndroid/tree/master/app/src/main/java/com/tejpratapsingh/recyclercalendaractivity/viewpager)
| ![Week Calender.gif](https://raw.githubusercontent.com/tejpratap46/RecyclerCalendarAndroid/master/sample_images/week_example.gif) | ![Month With Events.gif](https://raw.githubusercontent.com/tejpratap46/RecyclerCalendarAndroid/master/sample_images/month_vertical.gif) | ![Month With Events.gif](https://raw.githubusercontent.com/tejpratap46/RecyclerCalendarAndroid/master/sample_images/progress_sample.gif) |

**Above sample are not the limit of this library, possiblities are endless as you can create custom view for each date as well as add custom Business Login on top of it.**
  
## Installation
NuGet
```
Install-Package RecyclerCalendarXamarin
```
## Features

Highly customizable calendar views
Support for horizontal and vertical layouts
Event marking capabilities
Multiple selection modes
Infinite scrolling option
Built using AndroidX RecyclerView

## Sample Calendar Types:

Week Calendar (Horizontal)
Month with Events (Vertical)
Month with Swipe Pages (ViewPager)

## Usage
Simple Calendar Implementation
Add to your layout:
 
```
<Com.TejPratapSingh.RecyclerCalendar.Views.SimpleRecyclerCalendarView
    android:id="@+id/calendarRecyclerView"
    android:layout_width="match_parent"
    android:layout_height="match_parent" />
``` 
Initialize in your Activity:
```C#
var calendarView = FindViewById<SimpleRecyclerCalendarView>(Resource.Id.calendarRecyclerView);

var date = DateTime.Now;

// Configure date range
var startDate = DateTime.Now;
var endDate = DateTime.Now.AddMonths(3);

var configuration = new SimpleRecyclerCalendarConfiguration(
    calenderViewType: RecyclerCalendarConfiguration.CalendarViewType.Vertical,
    calendarLocale: Locale.Default,
    includeMonthHeader: true,
    selectionMode: new SimpleRecyclerCalendarConfiguration.SelectionModeNone()
);

configuration.WeekStartOffset = RecyclerCalendarConfiguration.StartDayOfWeek.Monday;

calendarView.Initialize(
    startDate,
    endDate, 
    configuration,
    new DateSelectedListener((selectedDate) => {
        Toast.MakeText(
            this,
            $"Date Selected: {selectedDate:dd/MM/yyyy}",
            ToastLength.Long
        ).Show();
    })
);
``` 

------------
**Infinite Calendar Implementation**
Layout:
```xml
<Com.TejPratapSingh.RecyclerCalendar.Views.InfiniteRecyclerCalendarView
    android:id="@+id/calendarRecyclerView"
    android:layout_width="match_parent"
    android:layout_height="match_parent" />
```
Activity code:
```C#
var calendarView = FindViewById<InfiniteRecyclerCalendarView>(Resource.Id.calendarRecyclerView);

var configuration = new InfiniteRecyclerCalendarConfiguration(
    calenderViewType: RecyclerCalendarConfiguration.CalendarViewType.Vertical,
    calendarLocale: Locale.Default,
    includeMonthHeader: true,
    selectionMode: new InfiniteRecyclerCalendarConfiguration.SelectionModeNone()
);

configuration.WeekStartOffset = RecyclerCalendarConfiguration.StartDayOfWeek.Monday;

calendarView.Initialize(
    configuration,
    new DateSelectedListener((selectedDate) => {
        Toast.MakeText(
            this,
            $"Date Selected: {selectedDate:dd/MM/yyyy}",
            ToastLength.Long
        ).Show();
    })
);
```
## Custom Calendar Implementation
Create your adapter by extending RecyclerCalendarBaseAdapter:

Here is how you can create your own Calendar using **RecyclerCalendarAndroid**.
Create a RecyclerView Adapter which extends `RecyclerCalendarBaseAdapter`
```c#
public class CustomCalendarAdapter : RecyclerCalendarBaseAdapter
{
    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var view = LayoutInflater.From(parent.Context)
            .Inflate(Resource.Layout.item_calendar_custom, parent, false);
        return new CustomCalendarViewHolder(view);
    }

    public override void OnBindViewHolder(
        RecyclerView.ViewHolder holder, 
        int position, 
        RecyclerCalenderViewItem calendarItem)
    {
        var viewHolder = (CustomCalendarViewHolder)holder;
        
        if (calendarItem.IsHeader)
        {
            // Bind header
            BindHeaderItem(viewHolder, calendarItem);
        }
        else if (calendarItem.IsEmpty)
        {
            // Bind empty item
            BindEmptyItem(viewHolder);
        }
        else
        {
            // Bind date item
            BindDateItem(viewHolder, calendarItem);
        }
    }
}
```
#### Selection Modes

None
Single
Multiple
Range

#### Additional Features

Event marking
Custom date cell views
Header customization
Horizontal/Vertical layouts
ViewPager integration

#### Requirements

Xamarin.Android
AndroidX.RecyclerView
Android API 21+

#### Credits
This library is a C# port of RecyclerCalendarAndroid by Tej Pratap Singh. 
All credit for the original design and architecture goes to the original author.
## License
MIT License
