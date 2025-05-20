using System.Collections.Generic;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Classes
{
   public class ListClass : BaseClass, ICollectionClass
   {
      public override string Name => "List";

      public IObject Revert(IEnumerable<IObject> list) => List.NewList(list);

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         collectionMessages();
         messages["list"] = (obj, _) => function<List>(obj, l => l);

         messages["head".get()] = (obj, _) => function<List>(obj, l => someOf(l.Head));
         messages["tail".get()] = (obj, _) => function<List>(obj, l => l.Tail);
         messages["init".get()] = (obj, _) => function<List>(obj, l => l.Init);
         messages["last".get()] = (obj, _) => function<List>(obj, l => someOf(l.Last));
         messages["~"] = (obj, msg) => function<List, List>(obj, msg, (l1, l2) => l1.Concatenate(l2));
         messages["isEmpty".get()] = (obj, _) => function<List>(obj, l => KBoolean.BooleanObject(l.IsEmpty));
         messages["[]"] = (obj, msg) => function<List, Int>(obj, msg, (l, i) => l[i.Value]);
      }

      public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");
   }
}