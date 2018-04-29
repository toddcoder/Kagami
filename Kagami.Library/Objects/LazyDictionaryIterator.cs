using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Objects
{
   public class LazyDictionaryIterator : LazyIterator
   {
      protected IObject[] keys;
      protected Dictionary dictionary;

      public LazyDictionaryIterator(ICollection collection) : base(collection)
      {
         dictionary = (Dictionary)collection;
         keys = dictionary.KeyArray;
      }

      public override IMaybe<IObject> Next() => when(index++ < keys.Length, () => dictionary[keys[index]]);

      public override IMaybe<IObject> Peek() => when(index < keys.Length, () => dictionary[keys[index]]);
   }
}