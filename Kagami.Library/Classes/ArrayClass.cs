using System.Collections.Generic;
using Kagami.Library.Objects;
using Standard.Types.Exceptions;
using Standard.Types.Maybe;
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
         messages["array"] = (obj, msg) => function<Array>(obj, a => a);
         mutableCollectionMessages();
         sliceableMessages();

         messages["[]"] = (obj, msg) => function<Array, Int>(obj, msg, (a, i) => a[i.Value]);
         messages["[]="] = (obj, msg) => function<Array>(obj, a => a[((Int)msg.Arguments[0]).Value] = msg.Arguments[1]);
         messages["indexed"] = (obj, msg) => function<Array>(obj, a => a.IndexedValues);
         messages["~"] = (obj, msg) => function<Array, Array>(obj, msg, (a1, a2) => a1.Concatenate(a2));
         registerMessage("push", (obj, msg) => function<Array, IObject>(obj, msg, (a, v) => a.Append(v)));
         registerMessage("pop", (obj, msg) => function<Array>(obj, a => a.Pop()));
         registerMessage("unshift", (obj, msg) => function<Array, IObject>(obj, msg, (a, v) => a.Unshift(v)));
         registerMessage("shift", (obj, msg) => function<Array>(obj, a => a.Shift()));
         messages["find".Function("", "startAt", "reverse")] = (obj, msg) =>
            function<Array, IObject, Int, Boolean>(obj, msg, (a, o, i, r) => a.Find(o, i.Value, r.Value));
         messages["find".Function("", "startAt")] = (obj, msg) =>
            function<Array, IObject, Int>(obj, msg, (a, o, i) => a.Find(o, i.Value, false));
         messages["find".Function("", "reverse")] = (obj, msg) =>
            function<Array, IObject, Boolean>(obj, msg, (a, o, r) => a.Find(o, 0, r.Value));
         messages["find"] = (obj, msg) =>
            function<Array, IObject>(obj, msg, (a, o) => a.Find(o, 0, false));
         messages["find".Function("all")] = (obj, msg) =>
            function<Array, IObject>(obj, msg, (a, o) => a.FindAll(o));
      }

      public override void RegisterClassMessages()
      {
         base.RegisterClassMessages();

         classMessages["repeat".Function("value", "times")] = (bc, msg) =>
            classFunc<ArrayClass, IObject, Int>(bc, msg, (ac, v, t) => Array.Repeat(v, t.Value));
         classMessages["empty".get()] = (bc, msg) => classFunc<ArrayClass>(bc, ac => Array.Empty);
         classMessages["typed"] = (bc, msg) => getTypedArray(msg);
      }

      static Array getTypedArray(Message message)
      {
         if (message.Arguments[0] is TypeConstraint typeConstraint)
            return new Array(new IObject[0]) { TypeConstraint = typeConstraint.Some() };
         else
            throw "Type constraint for array required".Throws();
      }
   }
}