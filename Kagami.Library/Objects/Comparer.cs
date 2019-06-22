using System;
using System.Collections;

namespace Kagami.Library.Objects
{
   public class Comparer : IComparer
   {
      Func<object, object, int> function;

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

      public int Compare(object x, object y) => function(x, y);
   }
}