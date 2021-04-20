using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public class Cycle : IObject, ICollection
   {
      public static IObject CreateObject(IEnumerable<IObject> items) => new Cycle(items.ToArray());

      protected IObject[] items;
      protected IMaybe<(IObject, Lambda)> _seedLambda;

      public Cycle(params IObject[] items)
      {
         this.items = items;
         if (this.items.Length == 2 && this.items[1] is Lambda lambda)
         {
            _seedLambda = (this.items[0], lambda).Some();
         }
         else
         {
            _seedLambda = none<(IObject, Lambda)>();
         }
      }

      public IMaybe<(IObject, Lambda)> SeedLambda => _seedLambda;

      public string ClassName => "Cycle";

      public string AsString => $"?({items.Select(i => i.AsString).ToString(", ")})";

      public string Image => $"?({items.Select(i => i.Image).ToString(", ")})";

      public int Hash => items.GetHashCode();

      public bool IsEqualTo(IObject obj) => isEqualTo(this, obj);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => items.Length > 0;

      public IIterator GetIterator(bool lazy) => new CycleCollection(this).GetIterator(lazy);

      public IMaybe<IObject> Next(int index)
      {
         var value = items[index % items.Length];
         if (value is Lambda lambda)
         {
            value = lambda.Invoke();
         }

         return value.Some();
      }

      public IMaybe<IObject> Peek(int index) => Next(index);

      public Int Length => items.Length;

      public bool ExpandForArray => true;

      public Boolean In(IObject item) => items.Contains(item);

      public Boolean NotIn(IObject item) => !items.Contains(item);

      public IObject Times(int count) => GetIterator(false).Take(count);

      public String MakeString(string connector) => makeString(this, connector);

      public IIterator GetIndexedIterator() => new IndexedIterator(this);

      public Tuple Items => new(items);

      public IObject this[int index] => items[index];

      public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
   }
}