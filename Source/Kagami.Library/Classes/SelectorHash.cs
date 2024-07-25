using System.Collections.Generic;
using Kagami.Library.Objects;
using Core.Collections;

namespace Kagami.Library.Classes
{
   public class SelectorHash<TValue> : Hash<string, TValue>
   {
      protected AutoHash<string, List<string>> buckets;

      public SelectorHash() => buckets = new AutoHash<string, List<string>>(_ => new List<string>(), true);

      public TValue this[Selector selector]
      {
         get
         {
            if (base.ContainsKey(selector.Image))
            {
               return base[selector.Image];
            }
            else
            {
               var labelsOnlyImage = selector.LabelsOnly().Image;
               if (buckets.ContainsKey(labelsOnlyImage))
               {
                  foreach (var bucket in buckets[labelsOnlyImage])
                  {
                     Selector matchSelector = bucket;
                     if (selector.IsEquivalentTo(matchSelector))
                     {
                        return base[matchSelector.Image];
                     }
                  }
               }

               return base[labelsOnlyImage];
            }
         }
         set
         {
            base[selector.Image] = value;
            buckets[selector.LabelsOnly()].Add(selector.Image);
         }
      }

      public bool ContainsKey(Selector selector)
      {
         if (base.ContainsKey(selector.Image))
         {
            return true;
         }
         else
         {
            var labelsOnlyImage = selector.LabelsOnly().Image;
            if (buckets.ContainsKey(labelsOnlyImage))
            {
               foreach (var bucket in buckets[labelsOnlyImage])
               {
                  Selector matchSelector = bucket;
                  if (selector.IsEquivalentTo(matchSelector))
                  {
                     return true;
                  }
               }
            }

            return base.ContainsKey(labelsOnlyImage);
         }
      }

      public bool ContainsExact(Selector selector) => base.ContainsKey(selector.Image);
   }
}