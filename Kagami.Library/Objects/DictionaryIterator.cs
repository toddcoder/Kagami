using System.Collections.Generic;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class DictionaryIterator : Iterator
{
   protected IObject[] keys;
   protected Dictionary dictionary;

   public DictionaryIterator(ICollection collection) : base(collection)
   {
      dictionary = (Dictionary)collection;
      keys = dictionary.KeyArray;
   }

   public override Maybe<IObject> Next() => maybe(index < keys.Length, () =>
   {
      var key = keys[index++];
      return KTuple.NewTupleNamed("key", key, "value", dictionary.GetRaw(key)).Some();
   });

   public override Maybe<IObject> Peek() => maybe(index < keys.Length, () =>
   {
      var key = keys[index];
      return KTuple.NewTupleNamed("key", key, "value", dictionary.GetRaw(key)).Some();
   });

   public override IEnumerable<IObject> List()
   {
      Maybe<IObject> _item = nil;
      index = 0;
      do
      {
         _item = Next();
         if (_item is (true, var item))
         {
            yield return item;
         }

         if (index % 1000 == 0 && Machine.Current.Context.Cancelled())
         {
            yield break;
         }
      } while (_item);
   }
}