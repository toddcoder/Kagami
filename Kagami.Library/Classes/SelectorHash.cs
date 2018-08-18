using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Collections;

namespace Kagami.Library.Classes
{
   public class SelectorHash<TValue> : Hash<string, TValue>
   {
      public TValue this[Selector key]
      {
         get
         {
            if (base.ContainsKey(key.Image))
               return base[key.Image];
            else
            {
               var count = key.SelectorItems.Length;
               var iterator = new BitIterator(count);
               foreach (var bools in iterator)
               {
                  var newSelector = key.Equivalent(bools);
                  if (base.ContainsKey(newSelector))
                     return base[newSelector.Image];
               }

               var labelsOnly = key.LabelsOnly();

               return base[labelsOnly.Image];
            }
         }
         set => base[key.Image] = value;
      }

      public bool ContainsKey(Selector selector)
      {
         if (base.ContainsKey(selector.Image))
            return true;
         else
         {
            var count = selector.SelectorItems.Length;
            var iterator = new BitIterator(count);
            foreach (var bools in iterator)
            {
               var newSelector = selector.Equivalent(bools);
               if (base.ContainsKey(newSelector))
                  return true;
            }

            var labelsOnly = selector.LabelsOnly();

            return base.ContainsKey(labelsOnly.Image);
         }
      }
   }
}