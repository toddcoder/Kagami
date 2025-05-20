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

   public override Maybe<IObject> Next()
   {
      if (index < keys.Length)
      {
         var key = keys[index++];
         return KTuple.NewTupleNamed("key", key, "value", dictionary.GetRaw(key)).Some();
      }
      else
      {
         return nil;
      }
   }

   public override Maybe<IObject> Peek()
   {
      if (index < keys.Length)
      {
         var key = keys[index];
         return KTuple.NewTupleNamed("key", key, "value", dictionary.GetRaw(key)).Some();
      }
      else
      {
         return nil;
      }
   }

   public override IEnumerable<IObject> List()
   {
      index = 0;
      do
      {
         var _item = Next();
         if (_item is (true, var item))
         {
            yield return item;
         }
         else
         {
            break;
         }

         if (index % 1000 == 0 && Machine.Current.Value.Context.Cancelled())
         {
            yield break;
         }
      } while (true);
   }
}