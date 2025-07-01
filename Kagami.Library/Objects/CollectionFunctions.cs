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
      Regex r => r.MatchesIndex(obj, intGetter),
      Sequence container => listGetter(obj, conditionContainer(container)),
      ICollection collection and not KString => listGetter(obj, new Sequence(collection.GetIterator(false).List())),
      IIterator iterator => listGetter(obj, new Sequence(iterator.List())),
      _ => throw invalidIndex(index)
   };

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
}