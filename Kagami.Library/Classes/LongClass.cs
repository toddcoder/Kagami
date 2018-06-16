using System.Numerics;
using Kagami.Library.Objects;

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
      }

      public IObject Parse(string source) => Long.LongObject(BigInteger.Parse(source));
   }
}