using Core.Enumerables;
using Core.Numbers;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public static class CollectionFunctions
{
   public static bool isEqualTo<T>(T left, IObject right) where T : ICollection
   {
      if (right is T other)
      {
         var l1 = left.GetIterator(false).List().ToArray();
         var l2 = other.GetIterator(false).List().ToArray();
         return l1.Length == l2.Length && l1.Zip(l2, (a, b) => a.IsEqualTo(b)).All(b => b);
      }
      else
      {
         return false;
      }
   }

   public static int compareCollections<T>(T left, IObject right) where T : ICollection, IObjectCompare
   {
      if (right is T other)
      {
         var l1 = left.GetIterator(false).List().ToArray();
         var l2 = other.GetIterator(false).List().ToArray();
         var length1 = l1.Length;
         var length2 = l2.Length;
         var length = Math.Min(length1, length2);
         for (var i = 0; i < length; i++)
         {
            var item1 = l1[i];
            var item2 = l2[i];
            if (item1 is IObjectCompare oc)
            {
               var compare = oc.Compare(item2);
               if (compare != 0)
               {
                  return compare;
               }
            }
            else
            {
               throw incompatibleClasses(item1, "Object compare");
            }
         }

         if (length1 == length2)
         {
            return 0;
         }
         else if (length1 < length2)
         {
            return -1;
         }
         else
         {
            return 1;
         }
      }
      else
      {
         throw incompatibleClasses(right, typeof(T).Name);
      }
   }

   public static KString makeString(ICollection collection, string connector)
   {
      return collection.GetIterator(false).List().Select(i => i.AsString).ToString(connector);
   }

   public static IEnumerable<int> indexList(Sequence sequence, int length)
   {
      return sequence.List
         .Cast<Int>()
         .Select(i => wrapIndex(i.Value, length))
         .Where(i => i.Between(0).Until(length));
   }

   public static IEnumerable<int> indexList(KRange range, int length)
   {
      var start = wrapIndex(((Int)range.Start).Value, length);
      var stop = wrapIndex(((Int)range.Stop).Value, length);
      return new KRange((Int)start, (Int)stop, range.Inclusive, range.Increment).GetIterator(false).List().Cast<Int>().Select(i => i.Value);
   }

   private static Sequence conditionContainer(Sequence sequence)
   {
      var list = new List<IObject>();
      foreach (var obj in sequence.List)
      {
         switch (obj)
         {
            case ICollection collection:
               foreach (var obj2 in collection.GetIterator(false).List())
               {
                  list.Add(obj2);
               }

               break;
            case IIterator iterator:
               foreach (var innerObject in iterator.List())
               {
                  list.Add(innerObject);
               }

               break;
            default:
               list.Add(obj);
               break;
         }
      }

      return new Sequence(list);
   }

   public static IObject getIndexed(IObject obj, IObject index, Func<IObject, int, IObject> intGetter,
      Func<IObject, Sequence, IObject> listGetter) => index switch
   {
      Int i => intGetter(obj, i.Value),
      KRange range => listGetter(obj, fromRange(range, obj)),
      Regex r => r.MatchesIndex(obj, intGetter),
      Sequence container => listGetter(obj, conditionContainer(container)),
      ICollection collection and not KString => listGetter(obj, new Sequence(collection.GetIterator(false).List())),
      IIterator iterator => listGetter(obj, new Sequence(iterator.List())),
      _ => throw invalidIndex(index)
   };

   private static Sequence fromRange(KRange range, IObject obj)
   {
      if (obj is ICollection collection)
      {
         var length = collection.Length.Value;
         var start = wrapIndex(((Int)range.Start).Value, length);
         var stop = wrapIndex(((Int)range.Stop).Value, length);

         return new KRange((Int)start, (Int)stop, range.Inclusive, range.Increment).GetIterator(false).Seq();
      }
      else
      {
         throw incompatibleClasses(obj, "Collection");
      }
   }

   public static void setIndexed(IObject obj, IObject index, IObject value, Action<IObject, int, IObject> intSetter,
      Action<IObject, Sequence, IObject> listSetter)
   {
      switch (index)
      {
         case Int i:
            intSetter(obj, i.Value, value);
            return;
         case Regex r:
            r.MatchesIndex(obj, intSetter, value);
            break;
         case Sequence container:
            listSetter(obj, conditionContainer(container), value);
            return;
         case ICollection collection and not KString:
            listSetter(obj, new Sequence(collection.GetIterator(false).List()), value);
            return;
         case IIterator iterator:
            listSetter(obj, new Sequence(iterator.List()), value);
            return;
         default:
            throw invalidIndex(index);
      }
   }

   public static IObject skipTake(ICollection collection, SkipTake skipTake)
   {
      var skipIterator = collection.GetIterator(true);
      var (skip, take) = skipTake;
      var takeIterator = (IIterator)skipIterator.Skip(skip);

      return takeIterator.Take(take);
   }

   public static IObject[] spread(IObject obj) => obj switch
   {
      ICollection collection => collection.GetIterator(false).List().ToArray(),
      IIterator iterator => iterator.List().ToArray(),
      _ => [obj]
   };

   public static IObject binarySearch(ICollection collection, IObject item)
   {
      IObject[] list = [..collection.GetIterator(false).List()];
      IObjectCompare[] compareList = [..list.OfType<IObjectCompare>()];
      if (list.Length != compareList.Length)
      {
         throw incompatibleClasses(item, "Object compare");
      }

      var left = 0;
      var right = list.Length - 1;
      while (left <= right)
      {
         var mid = left + (right - left) / 2;
         var compare = compareList[mid].Compare(item);
         switch (compare)
         {
            case 0:
               return Some.Object((Int)mid);
            case < 0:
               left = mid + 1;
               break;
            default:
               right = mid - 1;
               break;
         }
      }

      return KNil.NilValue;
   }

   public static IObject binarySearch(ICollection collection, IObject item, Lambda lambda)
   {
      IObject[] list = [.. collection.GetIterator(false).List()];

      var left = 0;
      var right = list.Length - 1;
      while (left <= right)
      {
         var mid = left + (right - left) / 2;
         var compare = lambda.Invoke(list[mid], item);
         if (compare is Int index)
         {
            switch (index.Value)
            {
               case 0:
                  return Some.Object((Int)mid);
               case < 0:
                  left = mid + 1;
                  break;
               default:
                  right = mid - 1;
                  break;
            }
         }
      }

      return KNil.NilValue;
   }

   public static IEnumerable<IObject> getEnumerable(IObject value) => value switch
   {
      ICollection collection => getEnumerable((IObject)collection.GetIterator(true)),
      IIterator iterator => iterator.List(),
      _ => new List<IObject>([value])
   };
}