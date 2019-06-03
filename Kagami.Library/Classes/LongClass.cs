using System.Numerics;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class LongClass : BaseClass, IParse
   {
      public override string Name => "Long";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         numericMessages();
         numericConversionMessages();

         messages["factorial()"] = (obj, msg) => function<Long>(obj, l => l.Factorial());
      }

      public IObject Parse(string source) => Long.LongObject(BigInteger.Parse(source));
   }
}