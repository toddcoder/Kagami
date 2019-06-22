using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Drawing
{
   public static class DrawingFunctions
   {
      public static IObject add(IObject x, IObject y)
      {
         return apply(x, y, (a, b) => a + b, (a, b) => a + b, (a, b) => a + b, (a, b) => a.Add(b), "+");
      }

      public static IObject subtract(IObject x, IObject y)
      {
         return apply(x, y, (a, b) => a - b, (a, b) => a - b, (a, b) => a - b, (a, b) => a.Subtract(b), "-");
      }

      public static int compare(IObject x, IObject y)
      {
         if (x is IObjectCompare xc)
         {
	         return xc.Compare(y);
         }
         else
         {
	         throw unableToConvert(x.Image, "ObjectCompare");
         }
      }
   }
}