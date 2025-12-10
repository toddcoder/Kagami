using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class DateIncrementClass : BaseClass
{
   public override string Name => "DateIncrement";

   public override IObject DefaultValue => new DateIncrement(new Date(DateTime.MinValue), 0);

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messages["date".get()] = (obj, _) => function<DateIncrement>(obj, di => di.Date);
      messages["amount".get()] = (obj, _) => function<DateIncrement>(obj, di => (Int)di.Amount);
      messages["microsecond".get()] = (obj, _) => function<DateIncrement>(obj, d => d.Microsecond());
      messages["millisecond".get()] = (obj, _) => function<DateIncrement>(obj, d => d.Millisecond());
      messages["second".get()] = (obj, _) => function<DateIncrement>(obj, d => d.Second());
      messages["minute".get()] = (obj, _) => function<DateIncrement>(obj, d => d.Minute());
      messages["hour".get()] = (obj, _) => function<DateIncrement>(obj, d => d.Hour());
      messages["day".get()] = (obj, _) => function<DateIncrement>(obj, d => d.Day());
      messages["month".get()] = (obj, _) => function<DateIncrement>(obj, d => d.Month());
      messages["year".get()] = (obj, _) => function<DateIncrement>(obj, d => d.Year());
   }
}