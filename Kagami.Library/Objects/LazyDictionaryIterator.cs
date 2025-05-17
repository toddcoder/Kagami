using System.Collections.Generic;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class LazyDictionaryIterator : LazyIterator
{
   protected IObject[] keys;
   protected Dictionary dictionary;

   public LazyDictionaryIterator(ICollection collection) : base(collection)
   {
      dictionary = (Dictionary)collection;
      keys = dictionary.KeyArray;
   }

   public override Maybe<IObject> Next() => maybe<IObject>() & index < keys.Length & (() =>
   {
      var key = keys[index++];
      return Tuple.NewTuple(key, dictionary.GetRaw(key)).Some();
   });

   public override Maybe<IObject> Peek() => maybe<IObject>() & index < keys.Length & (() =>
   {
      var key = keys[index];
      return Tuple.NewTuple(key, dictionary.GetRaw(key)).Some();
   });

   public override IEnumerable<IObject> List()
   {
      index = 0;
      do
      {
         var _item = Next();
         if (_item is (true, var obj))
         {
            yield return obj;
         }
         else
         {
            break;
         }

         if (index % 1000 == 0 && Machine.Current.Context.Cancelled())
         {
            yield break;
         }
      } while (true);
   }
}