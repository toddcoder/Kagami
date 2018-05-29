using System.Collections.Generic;
using Kagami.Library.Classes;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Objects
{
   public class ObjectCollection : IObject, ICollection
   {
      UserObject obj;
      BaseClass cls;

      public ObjectCollection(UserObject obj)
      {
         this.obj = obj;
         cls = classOf(this.obj);
      }

      public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

      public IMaybe<IObject> Next(int index)
      {
         var result = sendMessage(obj, "next", (Int)index);
         if (result is Some some)
            return some.Value.Some();
         else
            return none<IObject>();
      }

      public IMaybe<IObject> Peek(int index)
      {
         if (cls.RespondsTo("peek"))
         {
            var result = sendMessage(obj, "next", (Int)index);
            if (result is Some some)
               return some.Value.Some();
            else
               return none<IObject>();
         }
         else
            return none<IObject>();
      }

      public Int Length
      {
         get
         {
            if (cls.RespondsTo("length".get()))
               return (Int)sendMessage(obj, "length".get());
            else
               return -1;
         }
      }

      public IEnumerable<IObject> List
      {
         get
         {
            var i = 0;
            while (true)
               if (Next(i++).If(out var value))
                  yield return value;
               else
                  yield break;
         }
      }

      public bool ExpandForArray => true;

      public Boolean In(IObject item)
      {
         if (cls.RespondsTo("in"))
            return (Boolean)sendMessage(obj, "in", item);
         else
            return false;
      }

      public Boolean NotIn(IObject item)
      {
         if (cls.RespondsTo("notIn"))
            return (Boolean)sendMessage(obj, "notIn", item);
         else
            return false;
      }

      public IObject Times(int count)
      {
         if (cls.RespondsTo("*"))
            return (Boolean)sendMessage(obj, "*", (Int)count);
         else
            return obj;
      }

      public IObject Flatten() => flatten(this);

      public string ClassName => obj.ClassName;

      public string AsString => obj.AsString;

      public string Image => obj.Image;

      public int Hash => obj.Hash;

      public bool IsEqualTo(IObject obj) => this.obj.IsEqualTo(obj);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, (oc1, oc2) => oc1.IsEqualTo(oc2), bindings);

      public bool IsTrue => obj.IsTrue;
   }
}