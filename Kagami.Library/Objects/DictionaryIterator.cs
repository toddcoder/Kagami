﻿using System.Collections.Generic;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

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

      public override IMaybe<IObject> Next() => maybe(index < keys.Length, () =>
      {
         var key = keys[index++];
         return Tuple.NewTupleNamed("key", key, "value", dictionary.GetRaw(key)).Some();
      });

      public override IMaybe<IObject> Peek() => maybe(index < keys.Length, () =>
      {
         var key = keys[index];
         return Tuple.NewTupleNamed("key", key, "value", dictionary.GetRaw(key)).Some();
      });

      public override IEnumerable<IObject> List()
      {
         var item = none<IObject>();
         index = 0;
         do
         {
            item = Next();
            if (item.If(out var obj))
            {
	            yield return obj;
            }

            if (index % 1000 == 0 && Machine.Current.Context.Cancelled())
            {
	            yield break;
            }
         } while (item.IsSome);
      }
   }
}