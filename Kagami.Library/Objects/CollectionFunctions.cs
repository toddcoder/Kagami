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
            if (l1.Length == l2.Length)
            {
               var result = l1.Zip(l2, (a, b) => ((IObjectCompare)a).Compare(b)).ToArray();
               if (result.All(i => i == 0))
                  return 0;
               else if (result.All(i => i < 0))
                  return -1;
               else if (result.All(i => i > 0))
                  return 1;
               else
                  return result.FirstOrDefault(i => i != 0);
            }
            else
               return l1.Length.CompareTo(l2.Length);
         }
         else
            throw incompatibleClasses(right, typeof(T).Name);
      }
   }
}