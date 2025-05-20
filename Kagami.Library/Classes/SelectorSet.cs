using Kagami.Library.Objects;
using Core.Collections;

namespace Kagami.Library.Classes;

public class SelectorSet : Set<string>
{
   protected Memo<string, List<string>> buckets = new Memo<string, List<string>>.Function(_ => []);

   public void Add(Selector selector)
   {
      base.Add(selector);
      buckets[selector.LabelsOnly().Image].Add(selector);
   }

   public void AddRange(IEnumerable<Selector> selectors)
   {
      foreach (var selector in selectors)
      {
         Add(selector);
      }
   }

   public new void AddRange(IEnumerable<string> sources) => AddRange(sources.Select(s => (Selector)s));

   public bool Contains(Selector selector)
   {
      if (base.Contains(selector.Image))
      {
         return true;
      }
      else
      {
         var labelsOnlyImage = selector.LabelsOnly().Image;
         if (buckets[labelsOnlyImage].Any(matchSelector => selector.IsEquivalentTo((Selector)matchSelector)))
         {
            return true;
         }

         return base.Contains(labelsOnlyImage);
      }
   }
}