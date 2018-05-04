using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class RationalClass : BaseClass
   {
      public override string Name => "Rational";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messageNumberMessages();
         compareMessages();
         numericConversionMessages();

         registerMessage("numerator".get(), (obj, msg) => function<Rational>(obj, r => Long.Object(r.Numerator)));
         registerMessage("denominator".get(), (obj, msg) => function<Rational>(obj, r => Long.Object(r.Denominator)));
      }
   }
}