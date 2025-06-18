using Core.Collections;
using Core.Dates;
using Core.Dates.DateIncrements;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Interval : IObject, IRangeItem
{
   public static implicit operator Interval(TimeSpan timeSpan) => new(timeSpan);

   public static IObject IntervalObject(TimeSpan timeSpan) => new Interval(timeSpan);

   private readonly TimeSpan value;

   public Interval(TimeSpan value) : this() => this.value = value;

   public TimeSpan Value => value;

   public string ClassName => "Interval";

   public string AsString => value.ToString();

   public string Image => value.ToLongString(true);

   public int Hash => value.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Interval i && value == i.value;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => value > TimeSpan.MinValue;

   public Guid Id { get; init; } = Guid.NewGuid();

   public int Compare(IObject obj)
   {
      if (obj is Interval i)
      {
         return value.CompareTo(i.value);
      }
      else
      {
         throw incompatibleClasses(obj, "Interval");
      }
   }

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive)
   {
      if (min is Interval minInterval && max is Interval maxInterval)
      {
         return value >= minInterval.value && (inclusive ? value <= maxInterval.value : value < maxInterval.value);
      }
      else
      {
         return false;
      }
   }

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public IRangeItem Successor => (Interval)value.Add(1.Second());

   public IRangeItem Predecessor => (Interval)value.Add(-1.Second());

   public KRange Range() => new((Interval)value.Add(-value.Seconds.Seconds()), this, true);

   public Int Milliseconds => value.Milliseconds;

   public Float TotalMilliseconds => value.TotalMilliseconds;

   public Int Seconds => value.Seconds;

   public Float TotalSeconds => value.TotalSeconds;

   public Int Minutes => value.Minutes;

   public Float TotalMinutes => value.TotalMinutes;

   public Int Hours => value.Hours;

   public Float TotalHours => value.TotalHours;

   public Int Days => value.Days;

   public Float TotalDays => value.TotalDays;
}