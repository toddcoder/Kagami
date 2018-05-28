using System;
using System.Collections.Generic;
using System.Linq;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Exceptions;
using Standard.Types.Maybe;
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

      public Array(IEnumerable<IObject> objects)
      {
         list = objects.ToList();
         arrayID = uniqueObjectID();
      }

      public Array(IObject value)
      {
         list = new List<IObject> { value };
         arrayID = uniqueObjectID();
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

      public int CompareTo(Array other) => compareCollections(this, other);

      public bool Equals(Array other) => isEqualTo(this, other);

      public IObject this[int index]
      {
         get => list[wrapIndex(index, list.Count)];
         set
         {
            if (value is Array array && array.arrayID == arrayID)
               throw "Can't assign an array item to itself".Throws();

            index = wrapIndex(index, list.Count);
            if (value is Del)
               list.RemoveAt(index);
            else
               list[index] = value;
         }
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

      public void Add(IObject obj) => list.Add(obj);

      public IObject Append(IObject obj)
      {
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
         list.Insert(index, obj);
         return this;
      }

      public Array IndexedValues => new Array(list.Select((o, i) => (IObject)new Tuple(Int.IntObject(i), o)));

      public IObject Concatenate(Array array)
      {
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
   }
}