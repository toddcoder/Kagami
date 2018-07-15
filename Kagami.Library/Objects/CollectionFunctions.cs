using System.Linq;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Objects
{
   public static class CollectionFunctions
   {
      public static bool isEqualTo<T>(T left, IObject right) where T : ICollection
      {
         if (right is T other)
         {
            var l1 = left.GetIterator(false).List().ToArray();
            var l2 = other.GetIterator(false).List().ToArray();
            if (l1.Length == l2.Length)
               return l1.Zip(l2, (a, b) => a.IsEqualTo(b)).All(b => b);
            else
               return false;
         }
         else
            return false;
      }

      public static int compareCollections<T>(T left, IObject right) where T : ICollection, IObjectCompare
      {
         if (right is T other)
         {
            var l1 = left.GetIterator(false).List().ToArray();
            var l2 = other.GetIterator(false).List().ToArray();
            var length1 = l1.Length;
            var length2 = l2.Length;
            var length = System.Math.Min(length1, length2);
            for (var i = 0; i < length; i++)
            {
               var item1 = l1[i];
               var item2 = l2[i];
               if (item1 is IObjectCompare oc)
               {
                  var compare = oc.Compare(item2);
                  if (compare != 0)
                     return compare;
               }
               else
               {
                  throw incompatibleClasses(item1, "Object compare");
               }
            }

            if (length1 == length2)
               return 0;
            else if (length1 < length2)
               return -1;
            else
               return 1;
         }
         else
            throw incompatibleClasses(right, typeof(T).Name);
      }

   }
}