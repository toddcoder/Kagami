using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class BindingClass : BaseClass
   {
      public override string Name => "Binding";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["name".get()] = (obj, msg) => function<Binding>(obj, b => (String)b.Name);
         messages["value".get()] = (obj, msg) => function<Binding>(obj, b => b.Value);
      }
   }
}