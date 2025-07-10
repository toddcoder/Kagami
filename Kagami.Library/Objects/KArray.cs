using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class KArray : IObject, IObjectCompare, IComparable<KArray>, IEquatable<KArray>, IMutableCollection, ISliceable, IIndexed, IFindIndex
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

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) =>
      match(this, comparisand, (a1, a2) => equalifier(a1, a2, bindings), bindings);

   protected static bool equalifier(KArray a1, IObject a2, Hash<string, IObject> bindings) => a2 switch
   {
      KArray array when a1.Length.Value == 0 && array.Length.Value == 0 => true,
      KArray array when a1.Length.Value == array.Length.Value => a1.list.Zip(array.list, (i1, i2) => i1.Match(i2, bindings)).All(b => b),
      _ => false
   };

   public bool IsTrue => list.Count > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

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
         if (value is KNil)
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

   public IObject Get(int index) => index.Between(0).Until(list.Count) ? Some.Object(list[index]) : KNil.NilValue;

   public IObject this[Sequence sequence]
   {
      get
      {
         List<IObject> result = [];
         foreach (var index in indexList(sequence, list.Count))
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
               foreach (var index in indexList(sequence, list.Count))
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
               foreach (var index in indexList(sequence, list.Count))
               {
                  list[index] = value;
               }

               break;
            }
         }
      }
   }

   public IObject FromOpenRange(OpenRange openRange)
   {
      List<IObject> result = [];
      var iterator = openRange.GetIterator(true);
      var _item = iterator.Next();
      while (_item is (true, Int { Value: > -1 } i) && i.Value < list.Count)
      {
         result.Add(list[i.Value]);
         _item = iterator.Next();
      }

      return new KArray(result);
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

   public IObject One() => list.Count == 1 ? list[0] : this;

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
      if (obj is ICollection collection)
      {
         List<IObject> listToRemove = [..collection.GetIterator(false).List()];
         foreach (var item in listToRemove)
         {
            list.Remove(item);
         }
      }
      else
      {
         list.Remove(obj);
      }

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
      index = wrapIndex(index, list.Count);
      list.Insert(index, obj);

      return this;
   }

   public KBoolean IsEmpty => list.Count == 0;

   public KBoolean IsNotEmpty => list.Count > 0;

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

   public IObject Pop() => list.Count > 0 ? Some.Object(RemoveAt(list.Count - 1)) : KNil.NilValue;

   public IObject Unshift(IObject value) => InsertAt(0, value);

   public IObject Shift() => list.Count > 0 ? Some.Object(RemoveAt(0)) : KNil.NilValue;

   public IObject IndexOf(IObject item)
   {
      var index = list.IndexOf(item);
      if (index > -1)
      {
         return Some.Object((Int)index);
      }
      else
      {
         return KNil.NilValue;
      }
   }

   public IObject Index(Lambda predicate)
   {
      for (var i = 0; i < list.Count; i++)
      {
         var result = predicate.Invoke(list[i]);
         if (result.IsTrue)
         {
            return Some.Object((Int)i);
         }
      }

      return KNil.NilValue;
   }

   public IObject LastIndex(Lambda predicate)
   {
      for (var i = list.Count - 1; i >= 0; i--)
      {
         var result = predicate.Invoke(list[i]);
         if (result.IsTrue)
         {
            return Some.Object((Int)i);
         }
      }

      return KNil.NilValue;
   }

   public IObject LastIndexOf(IObject item)
   {
      var index = list.LastIndexOf(item);
      if (index > -1)
      {
         return Some.Object((Int)index);
      }
      else
      {
         return KNil.NilValue;
      }
   }

   public IObject FindAll(Lambda predicate)
   {
      List<IObject> found = [];
      foreach (var obj in list)
      {
         var result = predicate.Invoke(obj);
         if (result.IsTrue)
         {
            found.Add(obj);
         }
      }

      return new KArray(found);
   }

   public IObject First(Lambda lambda)
   {
      foreach (var item in list.Where(item => lambda.Invoke(item).IsTrue))
      {
         return Some.Object(item);
      }

      return KNil.NilValue;
   }

   public IObject Last(Lambda lambda)
   {
      for (var i = list.Count - 1; i > -1; i--)
      {
         if (lambda.Invoke(list[i]).IsTrue)
         {
            return Some.Object(list[i]);
         }
      }

      return KNil.NilValue;
   }

   public IObject BinarySearch(IObject item) => binarySearch(this, item);

   public IObject BinarySearch(IObject item, Lambda lambda) => binarySearch(this, item, lambda);

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

   public IObject Head => list.Count > 0 ? Some.Object(list[0]) : KNil.NilValue;

   public KArray Tail => list.Count > 0 ? new KArray([.. list.Skip(1)]) : new KArray([]);

   public KTuple HeadTail => new(Head, Tail);
}