using System.Collections;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class Comparer : IComparer
{
   protected Func<object, object, int> function;

   public Comparer(bool ascending)
   {
      if (ascending)
      {
         function = (x, y) =>
         {
            var comparer = new ObjectComparer();
            return comparer.Compare((IObject)x, (IObject)y);
         };
      }
      else
      {
         function = (x, y) =>
         {
            var comparer = new ObjectComparer();
            return comparer.Compare((IObject)y, (IObject)x);
         };
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
         return ((Int)classOf(x).SendMessage(x, "<>(_)", new Arguments(y))).Value;
      }
   }
}