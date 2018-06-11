using Kagami.Library;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Collections
{
   public class CollectionsClass : PackageClass
   {
      public override string Name => "Collections";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         registerPackageFunction("Set", (obj, msg) => function<Collections>(obj, c => c.Set(msg.Arguments.Value)));
         registerPackageFunction("Set".Function("of"), (obj, msg) => function<Collections>(obj, c => c.Set((ICollection)msg.Arguments[0])));
         registerPackageFunction("Set".Function("set"), (obj, msg) => function<Collections, Set>(obj, msg, (c, s) => c.Set(s)));
      }
   }
}