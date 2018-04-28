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

         messages["min".get()] = (cls, msg) => Int.Object(int.MinValue);
         messages["max".get()] = (cls, msg) => Int.Object(int.MaxValue);
         messages["parse"] = (cls, msg) => Int.Object(int.Parse(msg.Arguments[0].AsString));
      }
   }
}