using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Collections;

namespace Kagami.Library.Classes
{
   public class SelectorSet : Set<string>
   {
      public override void Add(string item)
      {
         if (!item.EndsWith(")"))
            item = $"{item}()";
         base.Add(item);
      }

      public bool Contains(Selector selector)
      {
         if (base.Contains(selector.Image))
            return true;
         else
         {
            var count = selector.SelectorItems.Length;
            var iterator = new BitIterator(count);
            foreach (var bools in iterator)
            {
               var newSelector = selector.Equivalent(bools);
               if (base.Contains(newSelector))
                  return true;
            }

            var labelsOnly = selector.LabelsOnly();

            return base.Contains(labelsOnly.Image);
         }
      }
   }
}