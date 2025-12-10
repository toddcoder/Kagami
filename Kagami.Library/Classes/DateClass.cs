using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class DateClass : BaseClass
{
   public override string Name => "Date";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      compareMessages();
      rangeMessages();

      messages["year".get()] = (obj, _) => function<Date>(obj, d => d.Year);
      messages["month".get()] = (obj, _) => function<Date>(obj, d => d.Month);
      messages["day".get()] = (obj, _) => function<Date>(obj, d => d.Day);
      messages["hour".get()] = (obj, _) => function<Date>(obj, d => d.Hour);
      messages["minute".get()] = (obj, _) => function<Date>(obj, d => d.Minute);
      messages["second".get()] = (obj, _) => function<Date>(obj, d => d.Second);
      messages["millisecond".get()] = (obj, _) => function<Date>(obj, d => d.Millisecond);
      messages["microsecond".get()] = (obj, _) => function<Date>(obj, d => d.Microsecond);
      messages["nanosecond".get()] = (obj, _) => function<Date>(obj, d => d.Nanosecond);
      messages["time".get()] = (obj, _) => function<Date>(obj, d => d.Time);
      messages["ticks".get()] = (obj, _) => function<Date>(obj, d => d.Ticks);
      messages["+(_)"] = (obj, msg) => function<Date, Interval>(obj, msg, (d, i) => d.Add(i));
      messages["-(_)"] = (obj, msg) => function<Date, IObject>(obj, msg, (d, i) => d.Subtract(i));
      messages["dayOfWeek".get()] = (obj, _) => function<Date>(obj, d => d.DayOfWeek);
      messages["dayOfYear".get()] = (obj, _) => function<Date>(obj, d => d.DayOfYear);
      messages["julian".get()] = (obj, _) => function<Date>(obj, d => d.Julian);
      messages["mjulian".get()] = (obj, _) => function<Date>(obj, d => d.MJulian);
      messages["utc()"] = (obj, _) => function<Date>(obj, d => d.Utc());
      messages["<<(_<Int>)"] = (obj, msg) => function<Date, Int>(obj, msg, (d, i) => d.Shift(i.Value));
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["now".get()] = (_, _) => new Date(DateTime.Now);
      classMessages["today".get()] = (_, _) => new Date(DateTime.Today);
      classMessages["min".get()] = (_, _) => new Date(DateTime.MinValue);
      classMessages["max".get()] = (_, _) => new Date(DateTime.MaxValue);
      classMessages["utcNow".get()] = (_, _) => new Date(DateTime.UtcNow);
      classMessages["parse(_)"] = (_, msg) => parse(msg.Arguments[0].AsString);
      classMessages["months".get()] = (_, _) => Date.Months;
      classMessages["daysOfTheWeek".get()] = (_, _) => Date.DaysOfTheWeek;
      classMessages["daysInMonth(year:_<Int>,month:_<Int>)"] =
         (bc, msg) => classFunc<DateClass, Int, Int>(bc, msg, (_, y, m) => daysInMonth(y.Value, m.Value));
      classMessages["daysInMonth".get()] = (_, _) => daysInMonth();
   }

   public override IObject DefaultValue => new Date(DateTime.MinValue);

   protected static IObject parse(string source)
   {
      try
      {
         return Success.Object((Date)DateTime.Parse(source));
      }
      catch (Exception exception)
      {
         return Failure.Object(exception.Message);
      }
   }

   protected static Int daysInMonth(int year, int month) => DateTime.DaysInMonth(month, month);

   protected static KArray daysInMonth()
   {
      IEnumerable<IObject> days = [.. ((int[])[31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]).Select(i => (Int)i)];
      return new KArray(days);
   }
}