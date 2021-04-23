using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class SkipTakeClass : BaseClass
   {
      public override string Name => "SkipTake";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         registerMessage("skip".get(), (obj, msg) => function<SkipTake, Int>(obj, msg, (st, _) => (Int)st.Skip));
         registerMessage("take".get(), (obj, msg) => function<SkipTake, Int>(obj, msg, (st, _) => (Int)st.Take));
      }
   }
}