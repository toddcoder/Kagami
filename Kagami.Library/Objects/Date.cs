using Core.Collections;
using Core.Dates;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Date : IObject, IRangeItem, IFormattable
{
   public static implicit operator Date(DateTime dateTime) => new(dateTime);

   public static IObject DateObject(DateTime dateTime) => new Date(dateTime);

   public static KTuple Months
   {
      get
      {
         string[] months =
         [
            "January", "February", "March", "April", "May", "June", "July", "August", "September", "October",
            "November", "December"
         ];
         var names = months.Select(KString.StringObject).ToArray();

         return new KTuple(names);
      }
   }

   public static KTuple DaysOfTheWeek
   {
      get
      {
         string[] days =
         [
            "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
         ];
         var names = days.Select(KString.StringObject).ToArray();

         return new KTuple(names);
      }
   }

   private const string SHORT_FORMAT = "MM/dd/yyyy HH:mm:ss";
   private const string LONG_FORMAT = "MM/dd/yyyy HH:mm:ss.ffff";

   private readonly DateTime value;

   public Date(DateTime value) : this() => this.value = value;

   public string ClassName => "Date";

   public string AsString => value.ToString(SHORT_FORMAT);

   public string Image => $"d\"{value.ToString(LONG_FORMAT)}\"";

   public int Hash => value.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Date d && value == d.value;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => value > DateTime.MinValue;

   public int Compare(IObject obj)
   {
      if (obj is Date d)
      {
         return value.CompareTo(d.value);
      }
      else
      {
         throw incompatibleClasses(obj, "Date");
      }
   }

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive)
   {
      if (min is Date minDate && max is Date maxDate)
      {
         return Compare(minDate) >= 0 && inclusive ? Compare(maxDate) <= 0 : Compare(maxDate) < 0;
      }
      else
      {
         return false;
      }
   }

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public Int Year => value.Year;

   public Int Month => value.Month;

   public Int Day => value.Day;

   public Int Hour => value.Hour;

   public Int Minute => value.Minute;

   public Int Second => value.Second;

   public Int Millisecond => value.Millisecond;

   public Long Ticks => new(value.Ticks);

   public IRangeItem Successor => new Date(value.AddDays(1));

   public IRangeItem Predecessor => new Date(value.AddDays(-1));

   public KRange Range() => new(new Date(value.Truncate()), this, true);

   public Date Add(Interval interval) => new(value.Add(interval.Value));

   public IObject Subtract(IObject obj) => obj switch
   {
      Interval i => new Date(value.Add(-i.Value)),
      Date d => new Interval(value - d.value),
      _ => throw incompatibleClasses(obj, "Interval or Date")
   };

   public KString Format(string format) => value.ToString(format);

   public KString DayOfWeek => value.DayOfWeek.ToString();
}