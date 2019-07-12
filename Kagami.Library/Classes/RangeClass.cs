using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class RangeClass : BaseClass, ICollectionClass
   {
      public override string Name => "Range";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         collectionMessages();

         messages["start".get()] = (obj, msg) => function<Range>(obj, r => r.StartObj);
         messages["stop".get()] = (obj, msg) => function<Range>(obj, r => r.StopObj);
         messages["increment".get()] = (obj, msg) => function<Range>(obj, r => (Int)r.Increment);
         messages["in"] = (obj, msg) => function<Range, IObject>(obj, msg, (r, o) => r.In(o));
         messages["notIn"] = (obj, msg) => function<Range, IObject>(obj, msg, (r, o) => r.NotIn(o));
         messages["+"] = (obj, msg) => function<Range, Int>(obj, msg, (r, i) => r.Add(i.Value));
         messages["-"] = (obj, msg) => function<Range, Int>(obj, msg, (r, i) => r.Subtract(i.Value));
         messages["inverse()"] = (obj, msg) => function<Range>(obj, r => r.Reverse());
         //messages["*"] = (obj, msg) => function<Range, Int>(obj, msg, (r, i) => r.Times(i.Value));
      }

      public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");

      public IObject Revert(IEnumerable<IObject> list) => new Array(list.ToList());
   }
}