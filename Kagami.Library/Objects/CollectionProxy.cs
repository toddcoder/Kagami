using System.Collections.Generic;
using System.Linq;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Objects
{
   public class CollectionProxy : IObject, ICollection
   {
      ICollection collection;
      IEnumerable<IObject> enumerable;
      IEnumerator<IObject> enumerator;

      public CollectionProxy(ICollection collection)
      {
         this.collection = collection;
         enumerable = this.collection.GetIterator(false).List();
         enumerator = enumerable.GetEnumerator();
      }

      public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

      public IMaybe<IObject> Next(int index)
      {
         if (enumerator.MoveNext())
            return enumerator.Current.Some();
         else
            return none<IObject>();
      }

      public IMaybe<IObject> Peek(int index) => enumerator.Current.SomeIfNotNull();

      public Int Length => enumerable.Count();

      public bool ExpandForArray => true;

      public Boolean In(IObject item) => collection.In(item);

      public Boolean NotIn(IObject item) => collection.NotIn(item);

      public IObject Times(int count) => collection.Times(count);

      public string ClassName => "CollectionProxy";

      public string AsString => enumerable.Select(i => i.AsString).Listify(" ");

      public string Image => enumerable.Select(i => i.Image).Listify();

      public int Hash => ((IObject)collection).Hash;

      public bool IsEqualTo(IObject obj) => obj is CollectionProxy cp && ((IObject)collection).IsEqualTo((IObject)cp.collection);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => false;

      public bool IsTrue => true;
   }
}