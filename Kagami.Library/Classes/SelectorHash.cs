using Kagami.Library.Objects;
using Core.Collections;

namespace Kagami.Library.Classes;

public class SelectorHash<TValue> : Hash<string, TValue> where TValue : notnull
{
   protected Memo<string, List<string>> buckets = new Memo<string, List<string>>.Function(_ => []);

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
            foreach (var matchSelector in buckets[labelsOnlyImage].Where(matchSelector => selector.IsEquivalentTo((Selector)matchSelector)))
            {
               return base[((Selector)matchSelector).Image];
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
         foreach (var bucket in buckets[labelsOnlyImage])
         {
            Selector matchSelector = bucket;
            if (selector.IsEquivalentTo(matchSelector))
            {
               return true;
            }
         }

         return base.ContainsKey(labelsOnlyImage);
      }
   }

   public bool ContainsExact(Selector selector) => base.ContainsKey(selector.Image);
}