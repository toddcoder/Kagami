using System;
using Core.Collections;
using Core.Dates;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct Date : IObject, IRangeItem, IFormattable
   {
      public static implicit operator Date(DateTime dateTime) => new Date(dateTime);

      public static IObject DateObject(DateTime dateTime) => new Date(dateTime);

      const string SHORT_FORMAT = "MM/dd/yyyy HH:mm:ss";
      const string LONG_FORMAT = "MM/dd/yyyy HH:mm:ss.ffff";

      DateTime value;

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
            return value.CompareTo(d.value);
         else
            throw incompatibleClasses(obj, "Date");
      }

      public IObject Object => this;

      public Boolean Between(IObject min, IObject max, bool inclusive)
      {
         if (min is Date minDate && max is Date maxDate)
            return Compare(minDate) >= 0 && inclusive ? Compare(maxDate) <= 0 : Compare(maxDate) < 0;
         else
            return false;
      }

      public Boolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

      public Int Year => value.Year;

      public Int Month => value.Month;

      public Int Day => value.Day;

      public Int Hour => value.Hour;

      public Int Minute => value.Minute;

      public Int Second => value.Second;

      public Int Millisecond => value.Millisecond;

      public Long Ticks => new Long(value.Ticks);

      public IRangeItem Successor => new Date(value.AddDays(1));

      public IRangeItem Predecessor => new Date(value.AddDays(-1));

      public Range Range() => new Range(new Date(value.Truncate()), this, true);

      public Date Add(Interval interval) => new Date(value.Add(interval.Value));

      public IObject Subtract(IObject obj)
      {
         switch (obj)
         {
            case Interval i:
               return new Date(value.Add(-i.Value));
            case Date d:
               return new Interval(value - d.value);
            default:
               throw incompatibleClasses(obj, "Interval or Date");
         }
      }

      public String Format(string format) => value.ToString(format);

	   public String DayOfWeek => value.DayOfWeek.ToString();
   }
}