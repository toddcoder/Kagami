using System;
using Core.Objects;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class IntClass : BaseClass, IParse, IEquivalentClass
{
   public override string Name => "Int";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      numericMessages();
      numericConversionMessages();
      rangeMessages();
      compareMessages();

      messages["isEven".get()] = (obj, _) => function<Int>(obj, i => i.IsEven);
      messages["isOdd".get()] = (obj, _) => function<Int>(obj, i => i.IsOdd);
      messages["isPrime".get()] = (obj, _) => function<Int>(obj, i => i.IsPrime);
      messages["factorial()"] = (obj, _) => function<Int>(obj, i => i.Factorial());
      messages["millisecond".get()] = (obj, _) => function<Int>(obj, i => i.Millisecond);
      messages["milliseconds".get()] = (obj, _) => function<Int>(obj, i => i.Millisecond);
      messages["second".get()] = (obj, _) => function<Int>(obj, i => i.Second);
      messages["seconds".get()] = (obj, _) => function<Int>(obj, i => i.Second);
      messages["minute".get()] = (obj, _) => function<Int>(obj, i => i.Minute);
      messages["minutes".get()] = (obj, _) => function<Int>(obj, i => i.Minute);
      messages["hour".get()] = (obj, _) => function<Int>(obj, i => i.Hour);
      messages["hours".get()] = (obj, _) => function<Int>(obj, i => i.Hour);
      messages["day".get()] = (obj, _) => function<Int>(obj, i => i.Day);
      messages["days".get()] = (obj, _) => function<Int>(obj, i => i.Day);
      messages["week".get()] = (obj, _) => function<Int>(obj, i => i.Week);
      messages["weeks".get()] = (obj, _) => function<Int>(obj, i => i.Week);
      messages["char()"] = (obj, _) => function<Int>(obj, i => i.Char());
      messages["byte()"] = (obj, _) => function<Int>(obj, i => i.Byte());
      messages["times(_)"] = (obj, msg) => function<Int, Lambda>(obj, msg, (i, l) => i.Times(l));
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["min".get()] = (_, _) => Int.IntObject(int.MinValue);
      classMessages["max".get()] = (_, _) => Int.IntObject(int.MaxValue);
      classMessages["parse"] = (_, msg) => parse(msg.Arguments[0].AsString);
   }

   public static IObject parse(string value)
   {
      try
      {
         var number = int.Parse(value.Replace("_", ""));
         return Success.Object(Int.IntObject(number));
      }
      catch (Exception exception)
      {
         return Failure.Object(exception.Message);
      }
   }

   public IObject Parse(string source) => Int.IntObject(source.Value().Int32());

   public override bool IsNumeric => true;

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Number");
}