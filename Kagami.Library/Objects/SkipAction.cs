using System.Collections.Generic;

namespace Kagami.Library.Objects
{
   public class SkipAction : IStreamAction
   {
      int count;
      int index;

      public SkipAction(int count)
      {
         this.count = count;
         index = -1;
      }

      public ILazyStatus Next(ILazyStatus status)
      {
         if (++index < count)
         {
	         return new Skipped();
         }
         else
         {
	         return status;
         }
      }

      public IEnumerable<IObject> Execute(IIterator iterator)
      {
         var i = -1;
         foreach (var value in iterator.List())
         {
	         if (++i >= count)
	         {
		         yield return value;
	         }
         }
      }

      public override string ToString() => $"skip {count}";
   }
}