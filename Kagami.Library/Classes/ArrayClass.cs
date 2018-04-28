using System.Collections.Generic;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class ArrayClass : BaseClass, ICollectionClass
   {
      public override string Name => "Array";

      public IObject Revert(IEnumerable<IObject> list) => new Array(list);

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         collectionMessages();
         mutableCollectionMessages();
         sliceableMessages();

         messages["[]"] = (obj, msg) => function<Array, Int>(obj, msg, (a, i) => a[i.Value]);
         messages["[]="] = (obj, msg) => function<Array>(obj, a => a[((Int)msg.Arguments[0]).Value] = msg.Arguments[1]);
         messages["indexed".get()] = (obj, msg) => function<Array>(obj, a => a.IndexedValues);
         messages["~"] = (obj, msg) => function<Array, Array>(obj, msg, (a1, a2) => a1.Concatenate(a2));
      }

      public override void RegisterClassMessages()
      {
         base.RegisterClassMessages();

         classMessages["repeat".Function("value", "times")] = (bc, msg) =>
            classFunc<ArrayClass, IObject, Int>(bc, msg, (ac, v, t) => Array.Repeat(v, t.Value));
      }
   }
}