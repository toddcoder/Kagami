using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class KArray : IObject, IObjectCompare, IComparable<KArray>, IEquatable<KArray>, IMutableCollection, ISliceable, IIndexed
{
   public static IObject CreateObject(IEnumerable<IObject> items)
   {
      var list = items.ToList();
      if (list.All(i => i is IKeyValue) && list.Count > 0)
      {
         return new Dictionary(list);
      }
      else
      {
         return new KArray(list);
      }
   }

   public static IObject Empty => new KArray([]);

   public static KArray Repeat(IObject value, int times)
   {
      var init = Enumerable.Repeat(value, times).ToList();
      return new KArray(init);
   }

   protected List<IObject> list;
   protected int arrayID = uniqueObjectID();
   protected Maybe<TypeConstraint> _typeConstraint = nil;
   protected Maybe<Lambda> _defaultLambda = nil;
   protected Maybe<IObject> _defaultValue = nil;

   public KArray(IEnumerable<IObject> objects)
   {
      list = [];
      foreach (var obj in objects)
      {
         if (obj is KRange range)
         {
            list.AddRange(range.GetIterator(false).List());
         }
         else
         {
            list.Add(obj);
         }
      }
   }

   public KArray(IObject value)
   {
      list = [value];
   }

   public string ClassName => "Array";

   public string AsString => list.Select(i => i.AsString).ToString(" ");

   public string Image => $"[{list.Select(i => i.Image).ToString(", ")}]";

   public int Hash => list.GetHashCode();

   public bool IsEqualTo(IObject obj) => isEqualTo(this, obj);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand,
      (a1, a2) => { return a1.Length.Value == a2.Length.Value && a1.list.Zip(a2.list, (i1, i2) => i1.Match(i2, bindings)).All(b => b); }, bindings);

   public bool IsTrue => list.Count > 0;

   public int Compare(IObject obj) => compareCollections(this, obj);

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public int CompareTo(KArray? other) => compareCollections(this, other!);

   public bool Equals(KArray? other) => isEqualTo(this, other!);

   public Maybe<TypeConstraint> TypeConstraint
   {
      get => _typeConstraint;
      set => _typeConstraint = value;
   }

   public Maybe<Lambda> DefaultLambda
   {
      get => _defaultLambda;
      set => _defaultLambda = value;
   }

   public Maybe<IObject> DefaultValue
   {
      get => _defaultValue;
      set => _defaultValue = value;
   }

   protected void assertType(IObject value)
   {
      if (_typeConstraint is (true, var typeConstraint) && !typeConstraint.Matches(classOf(value)))
      {
         throw incompatibleClasses(value, typeConstraint.AsString);
      }
   }

   public IObject this[int index]
   {
      get
      {
         var wrappedIndex = wrapIndex(index, list.Count);
         if (wrappedIndex.Between(0).Until(list.Count))
         {
            return list[wrappedIndex];
         }
         else if (_defaultLambda is (true, var lambda))
         {
            return lambda.Invoke(Int.IntObject(index));
         }
         else if (_defaultValue is (true, var value))
         {
            return value;
         }
         else
         {
            throw badIndex(wrappedIndex);
         }
      }
      set
      {
         throwIfSelf(value);

         var wrappedIndex = wrapIndex(index, list.Count);
         if (value is KUnit)
         {
            list.RemoveAt(wrappedIndex);
         }
         else
         {
            assertType(value);
            list[wrappedIndex] = value;
         }
      }
   }

   public IObject this[Container container]
   {
      get
      {
         var result = new List<IObject>();
         foreach (var index in indexList(container, list.Count))
         {
            result.Add(list[index]);
         }

         return new KArray(result);
      }
      set
      {
         switch (value)
         {
            case KArray array when array.arrayID == arrayID:
               return;
            case ICollection collection and not KString:
            {
               var valueIterator = collection.GetIterator(false);
               foreach (var index in indexList(container, list.Count))
               {
                  var _item = valueIterator.Next();
                  if (_item is (true, var item))
                  {
                     list[index] = item;
                  }
                  else
                  {
                     break;
                  }
               }

               break;
            }

            default:
            {
               foreach (var index in indexList(container, list.Count))
               {
                  list[index] = value;
               }

               break;
            }
         }
      }
   }

   protected void throwIfSelf(IObject value)
   {
      if (value is KArray array && array.arrayID == arrayID)
      {
         throw fail("Can't assign an array item to itself");
      }
   }

   public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

   public Maybe<IObject> Next(int index) => maybe<IObject>() & index < list.Count & (() => this[index]);

   public Maybe<IObject> Peek(int index) => Next(index);

   public Int Length => list.Count;

   public IEnumerable<IObject> List => list;

   public Slice Slice(ICollection collection) => new(this, collection.GetIterator(false).List().ToArray());

   public Maybe<IObject> Get(IObject index) => Next(((Int)index).Value);

   public IObject Set(IObject index, IObject value)
   {
      var intIndex = wrapIndex(((Int)index).Value, list.Count);
      assertType(value);
      list[intIndex] = value;

      return this;
   }

   public bool ExpandForArray => false;

   int ISliceable.Length => list.Count;

   public KBoolean In(IObject item) => list.Contains(item);

   public KBoolean NotIn(IObject item) => !list.Contains(item);

   public IObject Times(int count)
   {
      var result = new List<IObject>();
      for (var i = 0; i < count; i++)
      {
         result.AddRange(list);
      }

      return new KArray(result);
   }

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public void Add(IObject obj)
   {
      assertType(obj);
      list.Add(obj);
   }

   public IObject Append(IObject obj)
   {
      throwIfSelf(obj);
      assertType(obj);
      list.Add(obj);

      return this;
   }

   public IObject Remove(IObject obj)
   {
      list.Remove(obj);
      return this;
   }

   public IObject RemoveAt(int index)
   {
      index = wrapIndex(index, list.Count);
      var obj = this[index];
      list.RemoveAt(index);

      return obj;
   }

   public IObject RemoveAll(IObject obj)
   {
      list.RemoveAll(o => o.IsEqualTo(obj));
      return this;
   }

   public IObject InsertAt(int index, IObject obj)
   {
      throwIfSelf(obj);
      assertType(obj);
      list.Insert(index, obj);

      return this;
   }

   public KBoolean IsEmpty => list.Count == 0;

   public IObject Assign(SkipTake skipTake, IEnumerable<IObject> values)
   {
      var left = list.Take(skipTake.Skip);
      var right = list.Skip(skipTake.Skip + skipTake.Take);

      List<IObject> newList = [.. left, .. values, .. right];

      list = newList;
      return this;
   }

   public IObject Concatenate(KArray kArray)
   {
      if (_typeConstraint is (true, var typeConstraint))
      {
         if (kArray._typeConstraint is (true, var otherConstraint))
         {
            if (!typeConstraint.IsEqualTo(otherConstraint))
            {
               throw fail("Incompatible type constraints");
            }
         }
         else
         {
            throw fail("Expected type constraint in RHS array");
         }
      }
      else if (kArray._typeConstraint)
      {
         throw fail("RHS array has a type constraint");
      }

      var newList = new List<IObject>(list);
      newList.AddRange(kArray.list);

      return new KArray(newList);
   }

   public IObject Pop() => list.Count > 0 ? Some.Object(RemoveAt(list.Count - 1)) : None.NoneValue;

   public IObject Unshift(IObject value) => InsertAt(0, value);

   public IObject Shift() => list.Count > 0 ? Some.Object(RemoveAt(0)) : None.NoneValue;

   /*public IObject Find(IObject item, int startIndex, bool reverse)
   {
      var index = reverse ? list.LastIndexOf(item, startIndex) : list.IndexOf(item, startIndex);
      return index == -1 ? None.NoneValue : Some.Object((Int)index);
   }*/
   public IObject IndexOf(IObject item)
   {
      var index = list.IndexOf(item);
      if (index > -1)
      {
         return Some.Object((Int)index);
      }
      else
      {
         return None.NoneValue;
      }
   }

   public IObject ReverseIndexOf(IObject item)
   {
      var index = list.LastIndexOf(item);
      if (index > -1)
      {
         return Some.Object((Int)index);
      }
      else
      {
         return None.NoneValue;
      }
   }

   public IObject First(Lambda lambda)
   {
      foreach (var item in list.Where(item => lambda.Invoke(item).IsTrue))
      {
         return Some.Object((Int)item);
      }

      return None.NoneValue;
   }

   public IObject Last(Lambda lambda)
   {
      for (var i = list.Count - 1; i > -1; i--)
      {
         if (lambda.Invoke(list[i]).IsTrue)
         {
            return Some.Object((Int)list[i]);
         }
      }

      return None.NoneValue;
   }

   public IObject FindAll(IObject item)
   {
      List<IObject> result = [];
      var index = 0;
      while (index > -1)
      {
         index = list.IndexOf(item, index);
         if (index > -1)
         {
            result.Add((Int)index);
            index++;
         }
      }

      return new KTuple([.. result]);
   }

   public KArray Transpose()
   {
      if (list.All(i => i is KArray) && list.Count > 0)
      {
         var listOfLists = list.Select(i => ((KArray)i).list.ToArray()).ToArray();
         var minLength = listOfLists.Min(a => a.Length);
         var outerList = new List<IObject>();
         for (var i = 0; i < minLength; i++)
         {
            var innerList = new List<IObject>();
            foreach (var listOf in listOfLists)
            {
               innerList.Add(listOf[i]);
            }

            outerList.Add(new KArray(innerList));
         }

         return new KArray(outerList);
      }
      else
      {
         return this;
      }
   }

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
}