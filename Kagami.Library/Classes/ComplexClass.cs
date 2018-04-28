using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class ComplexClass : BaseClass
   {
      public override string Name => "Complex";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messageNumberMessages();
         compareMessages();

         messages["real".get()] = (obj, msg) => function<Complex>(obj, c => c.Real);
         messages["imaginary".get()] = (obj, msg) => function<Complex>(obj, c => c.Imaginary);
      }
   }
}