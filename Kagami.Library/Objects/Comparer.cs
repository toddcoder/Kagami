using System.Collections;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class Comparer : IComparer
{
   protected Func<object, object, int> function;

   public Comparer(bool ascending)
   {
      if (ascending)
      {
         function = (x, y) => ((IObjectCompare)x).Compare((IObject)y);
      }
      else
      {
         function = (x, y) => ((IObjectCompare)y).Compare((IObject)x);
      }
   }

   public int Compare(object? x, object? y) => function(x!, y!);
}

public class ObjectComparer : IComparer<IObject>
{
   public int Compare(IObject? x, IObject? y)
   {
      if (x is null || y is null)
      {
         throw fail("Can't compare");
      }

      if (x is IObjectCompare xCompare)
      {
         return xCompare.Compare(y);
      }
      else
      {
         throw fail("Doesn't implement object compare");
      }
   }
}