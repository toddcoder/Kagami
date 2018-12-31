using System;
using Kagami.Library.Objects;
using Standard.Types.Strings;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class IntClass : BaseClass, IParse, IEquivalentClass
   {
      public override string Name => "Int";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         numericMessages();
         numericConversionMessages();
         formatMessage<Int>();
         rangeMessages();
         compareMessages();

         messages["isEven".get()] = (obj, msg) => function<Int>(obj, i => i.IsEven);
         messages["isOdd".get()] = (obj, msg) => function<Int>(obj, i => i.IsOdd);
         messages["isPrime".get()] = (obj, msg) => function<Int>(obj, i => i.IsPrime);
         messages["factorial"] = (obj, msg) => function<Int>(obj, i => i.Factorial());
         messages["millisecond".get()] = (obj, msg) => function<Int>(obj, i => i.Millisecond);
         messages["milliseconds".get()] = (obj, msg) => function<Int>(obj, i => i.Millisecond);
         messages["second".get()] = (obj, msg) => function<Int>(obj, i => i.Second);
         messages["seconds".get()] = (obj, msg) => function<Int>(obj, i => i.Second);
         messages["minute".get()] = (obj, msg) => function<Int>(obj, i => i.Minute);
         messages["minutes".get()] = (obj, msg) => function<Int>(obj, i => i.Minute);
         messages["hour".get()] = (obj, msg) => function<Int>(obj, i => i.Hour);
         messages["hours".get()] = (obj, msg) => function<Int>(obj, i => i.Hour);
         messages["day".get()] = (obj, msg) => function<Int>(obj, i => i.Day);
         messages["days".get()] = (obj, msg) => function<Int>(obj, i => i.Day);
         messages["week".get()] = (obj, msg) => function<Int>(obj, i => i.Week);
         messages["weeks".get()] = (obj, msg) => function<Int>(obj, i => i.Week);
         messages["char()"] = (obj, msg) => function<Int>(obj, i => i.Char());
         messages["byte()"] = (obj, msg) => function<Int>(obj, i => i.Byte());
      }

      public override void RegisterClassMessages()
      {
         base.RegisterClassMessages();

         classMessages["min".get()] = (cls, msg) => Int.IntObject(int.MinValue);
         classMessages["max".get()] = (cls, msg) => Int.IntObject(int.MaxValue);
         classMessages["parse"] = (cls, msg) => parse(msg.Arguments[0].AsString);
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

      public IObject Parse(string source) => Int.IntObject(source.ToInt());

      public override bool IsNumeric => true;

      public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Number");
   }
}