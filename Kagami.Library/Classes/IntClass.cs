using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class IntClass : BaseClass
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
         if (int.TryParse(value.Replace("_", ""), out var number))
            return Some.Object(Int.IntObject(number));
         else
            return Nil.NilValue;
      }
   }
}