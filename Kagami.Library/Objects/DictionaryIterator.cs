using System.Collections.Generic;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Objects
{
   public class DictionaryIterator : Iterator
   {
      protected IObject[] keys;
      protected Dictionary dictionary;

      public DictionaryIterator(ICollection collection) : base(collection)
      {
         dictionary = (Dictionary)collection;
         keys = dictionary.KeyArray;
      }

      public override IMaybe<IObject> Next() => when(index < keys.Length, () =>
      {
         var key = keys[index++];
         return Tuple.NewTupleNamed("key", key, "value", dictionary[key]).Some();
      });

      public override IMaybe<IObject> Peek() => when(index < keys.Length, () =>
      {
         var key = keys[index];
         return Tuple.NewTupleNamed("key", key, "value", dictionary[key]).Some();
      });

      public override IEnumerable<IObject> List()
      {
         var item = none<IObject>();
         index = 0;
         do
         {
            item = Next();
            if (item.If(out var obj))
               yield return obj;
            if (index % 1000 == 0 && Machine.Current.Context.Cancelled())
               yield break;
         } while (item.IsSome);
      }
   }
}