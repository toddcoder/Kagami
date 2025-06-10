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
      messages["ticks".get()] = (obj, _) => function<Date>(obj, d => d.Ticks);
      messages["+(_)"] = (obj, msg) => function<Date, Interval>(obj, msg, (d, i) => d.Add(i));
      messages["-(_)"] = (obj, msg) => function<Date, IObject>(obj, msg, (d, i) => d.Subtract(i));
      messages["dayOfWeek".get()] = (obj, _) => function<Date>(obj, d => d.DayOfWeek);
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
   }

   protected static IObject parse(string source)
   {
      return DateTime.TryParse(source, out var dateTime) ? Some.Object((Date)dateTime) : None.NoneValue;
   }
}