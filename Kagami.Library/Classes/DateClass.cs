using System;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class DateClass : BaseClass
   {
      public override string Name => "Date";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         compareMessages();
         rangeMessages();
         formatMessage<Date>();

         messages["year".get()] = (obj, msg) => function<Date>(obj, d => d.Year);
         messages["month".get()] = (obj, msg) => function<Date>(obj, d => d.Month);
         messages["day".get()] = (obj, msg) => function<Date>(obj, d => d.Day);
         messages["hour".get()] = (obj, msg) => function<Date>(obj, d => d.Hour);
         messages["minute".get()] = (obj, msg) => function<Date>(obj, d => d.Minute);
         messages["second".get()] = (obj, msg) => function<Date>(obj, d => d.Second);
         messages["millisecond".get()] = (obj, msg) => function<Date>(obj, d => d.Millisecond);
         messages["ticks".get()] = (obj, msg) => function<Date>(obj, d => d.Ticks);
         messages["+"] = (obj, msg) => function<Date, Interval>(obj, msg, (d, i) => d.Add(i));
         messages["-"] = (obj, msg) => function<Date, IObject>(obj, msg, (d, i) => d.Subtract(i));
      }

      public override void RegisterClassMessages()
      {
         base.RegisterClassMessages();

         classMessages["now".get()] = (bc, msg) => new Date(DateTime.Now);
         classMessages["today".get()] = (bc, msg) => new Date(DateTime.Today);
         classMessages["min".get()] = (bc, msg) => new Date(DateTime.MinValue);
         classMessages["max".get()] = (bc, msg) => new Date(DateTime.MaxValue);
         classMessages["utcNow".get()] = (bc, msg) => new Date(DateTime.UtcNow);
         classMessages["parse"] = (bc, msg) => parse(msg.Arguments[0].AsString);
      }

      static IObject parse(string source)
      {
         if (DateTime.TryParse(source, out var dateTime))
            return Some.Object((Date)dateTime);
         else
            return Nil.NilValue;
      }
   }
}