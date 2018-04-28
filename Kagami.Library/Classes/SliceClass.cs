using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class SliceClass : BaseClass
   {
      public override string Name => "Slice";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         collectionMessages();

         messages["="] = (obj, msg) => function<Slice, IObject>(obj, msg, (s, o) => s.Assign(o));
      }
   }
}