using System;
using System.Collections.Generic;
using System.Linq;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Exceptions;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Objects
{
   public class Array : IObject, IObjectCompare, IComparable<Array>, IEquatable<Array>, IMutableCollection, ISliceable
   {
      public static IObject CreateObject(IEnumerable<IObject> items)
      {
         if (items.All(i => i is IKeyValue))
            return new Dictionary(items);
         else
            return new Array(items);
      }

      public static IObject Empty => new Array(new IObject[0]);

      public static Array Repeat(IObject value, int times)
      {
         var init = Enumerable.Repeat(value, times).ToList();
         return new Array(init);
      }

      List<IObject> list;
      int arrayID;
      IMaybe<TypeConstraint> typeConstraint;

      public Array(IEnumerable<IObject> objects)
      {
         list = objects.ToList();
         arrayID = uniqueObjectID();
         typeConstraint = none<TypeConstraint>();
      }

      public Array(IObject value)
      {
         list = new List<IObject> { value };
         arrayID = uniqueObjectID();
         typeConstraint = none<TypeConstraint>();
      }

      public string ClassName => "Array";

      public string AsString => list.Select(i => i.AsString).Listify(" ");

      public string Image => $"{{{list.Select(i => i.Image).Listify()}}}";

      public int Hash => list.GetHashCode();

      public bool IsEqualTo(IObject obj) => isEqualTo(this, obj);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, (a1, a2) =>
      {
         if (a1.Length.Value != a2.Length.Value)
            return false;
         else
            return a1.list.Zip(a2.list, (i1, i2) => i1.Match(i2, bindings)).All(b => b);
      }, bindings);

      public bool IsTrue => list.Count > 0;

      public int Compare(IObject obj) => compareCollections(this, obj);

      public IObject Object => this;

      public Boolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

      public Boolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

      public int CompareTo(Array other) => compareCollections(this, other);

      public bool Equals(Array other) => isEqualTo(this, other);

      public IMaybe<TypeConstraint> TypeConstraint
      {
         get => typeConstraint;
         set => typeConstraint = value;
      }

      void assertType(IObject value)
      {
         if (typeConstraint.If(out var tc) && !tc.Matches(classOf(value)))
            throw incompatibleClasses(value, tc.AsString);
      }

      public IObject this[int index]
      {
         get => list[wrapIndex(index, list.Count)];
         set
         {
            throwIfSelf(value);

            index = wrapIndex(index, list.Count);
            if (value is Del)
               list.RemoveAt(index);
            else
            {
               assertType(value);
               list[index] = value;
            }
         }
      }

      void throwIfSelf(IObject value)
      {
         if (value is Array array && array.arrayID == arrayID)
            throw "Can't assign an array item to itself".Throws();
      }

      public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

      public IMaybe<IObject> Next(int index) => when(index < list.Count, () => this[index]);

      public IMaybe<IObject> Peek(int index) => Next(index);

      public Int Length => list.Count;

      public IEnumerable<IObject> List => list;

      public Slice Slice(ICollection collection) => new Slice(this, collection.GetIterator(false).List().ToArray());

      public IMaybe<IObject> Get(IObject index) => Next(((Int)index).Value);

      public IObject Set(IObject index, IObject value)
      {
         var intIndex = wrapIndex(((Int)index).Value, list.Count);
         assertType(value);
         list[intIndex] = value;

         return this;
      }

      public bool ExpandForArray => false;

      int ISliceable.Length => list.Count;

      public Boolean In(IObject item) => list.Contains(item);

      public Boolean NotIn(IObject item) => !list.Contains(item);

      public IObject Times(int count)
      {
         var result = new List<IObject>();
         for (var i = 0; i < count; i++)
            result.AddRange(list);

         return new Array(result);
      }

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
         var obj = this[index];
         list.RemoveAt(index);

         return obj;
      }

      public IObject InsertAt(int index, IObject obj)
      {
         throwIfSelf(obj);
         assertType(obj);
         list.Insert(index, obj);

         return this;
      }

      public IObject Concatenate(Array array)
      {
         if (typeConstraint.If(out var thisConstraint))
         {
            if (array.typeConstraint.If(out var otherConstraint))
            {
               if (!thisConstraint.IsEqualTo(otherConstraint))
                  throw "Incompatible type constraints".Throws();
            }
            else
               throw "Expected type constraint in RHS array".Throws();
         }
         else if (array.typeConstraint.IsSome)
            throw "RHS array has a type constraint".Throws();

         var newList = new List<IObject>(list);
         newList.AddRange(array.list);

         return new Array(newList);
      }

      public IObject Pop()
      {
         if (list.Count > 0)
            return Some.Object(RemoveAt(list.Count - 1));
         else
            return Nil.NilValue;
      }

      public IObject Unshift(IObject value) => InsertAt(0, value);

      public IObject Shift()
      {
         if (list.Count > 0)
            return Some.Object(RemoveAt(0));
         else
            return Nil.NilValue;
      }

      public IObject Find(IObject item, int startIndex, bool reverse)
      {
         var index = reverse ? list.LastIndexOf(item, startIndex) : list.IndexOf(item, startIndex);

         if (index == -1)
            return Nil.NilValue;
         else
            return Some.Object((Int)index);
      }

      public IObject FindAll(IObject item)
      {
         var result = new List<IObject>();
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

         return new Tuple(result.ToArray());
      }
   }
}