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
         registerPackageFunction("Set".Selector("of:<Collection>"), (obj, msg) => function<Collections>(obj, c => c.Set((ICollection)msg.Arguments[0])));
         registerPackageFunction("Set".Selector("set:<Set>"), (obj, msg) => function<Collections, Set>(obj, msg, (c, s) => c.Set(s)));
         registerPackageFunction("sequence".Selector("count:<Int>", "factor:<Int>", "offset:<Int>"),
            (obj, msg) => function<Collections, Int, Int, Int>(obj, msg, (coll, c, f, o) => coll.Sequence(c.Value, f.Value, o.Value)));
      }
   }
}