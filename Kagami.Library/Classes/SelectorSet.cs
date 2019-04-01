using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Objects;
using Core.Collections;

namespace Kagami.Library.Classes
{
   public class SelectorSet : Set<string>
   {
	   AutoHash<string, List<string>> buckets;

	   public SelectorSet() => buckets = new AutoHash<string, List<string>>(key => new List<string>(), true);

	   public void Add(Selector selector)
      {
         base.Add(selector);
	      buckets[selector.LabelsOnly().Image].Add(selector);
      }

	   public void AddRange(IEnumerable<Selector> selectors)
	   {
		   foreach (var selector in selectors)
			   Add(selector);
	   }

	   public new void AddRange(IEnumerable<string> sources) => AddRange(sources.Select(s => (Selector)s));

	   public bool Contains(Selector selector)
      {
	      if (base.Contains(selector.Image))
		      return true;
	      else
	      {
		      var labelsOnlyImage = selector.LabelsOnly().Image;
				if (buckets.ContainsKey(labelsOnlyImage))
					foreach (var bucket in buckets[labelsOnlyImage])
					{
						Selector matchSelector = bucket;
						if (selector.IsEquivalentTo(matchSelector))
							return true;
					}

		      return base.Contains(labelsOnlyImage);
	      }
      }
   }
}