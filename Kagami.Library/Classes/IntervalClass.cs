using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class IntervalClass : BaseClass
   {
      public override string Name => "Interval";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         compareMessages();
         rangeMessages();

         messages["milliseconds".get()] = (obj, _) => function<Interval>(obj, i => i.Milliseconds);
         messages["totalMilliseconds".get()] = (obj, _) => function<Interval>(obj, i => i.TotalMilliseconds);
         messages["seconds".get()] = (obj, _) => function<Interval>(obj, i => i.Seconds);
         messages["totalSeconds".get()] = (obj, _) => function<Interval>(obj, i => i.TotalSeconds);
         messages["minutes".get()] = (obj, _) => function<Interval>(obj, i => i.Minutes);
         messages["totalMinutes".get()] = (obj, _) => function<Interval>(obj, i => i.TotalMinutes);
         messages["hours".get()] = (obj, _) => function<Interval>(obj, i => i.Hours);
         messages["totalHours".get()] = (obj, _) => function<Interval>(obj, i => i.TotalHours);
         messages["days".get()] = (obj, _) => function<Interval>(obj, i => i.Days);
         messages["totalDays".get()] = (obj, _) => function<Interval>(obj, i => i.TotalDays);
      }
   }
}