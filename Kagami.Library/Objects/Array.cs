﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;
using Core.Exceptions;
using Core.Monads;
using Core.Numbers;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Operations.OperationFunctions;

namespace Kagami.Library.Objects
{
   public class Array : IObject, IObjectCompare, IComparable<Array>, IEquatable<Array>, IMutableCollection, ISliceable
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
            return new Array(list);
         }
      }

      public static IObject Empty => new Array(new IObject[0]);

      public static Array Repeat(IObject value, int times)
      {
         var init = Enumerable.Repeat(value, times).ToList();
         return new Array(init);
      }

      protected List<IObject> list;
      protected int arrayID;
      protected IMaybe<TypeConstraint> _typeConstraint;
      protected IMaybe<Lambda> _defaultLambda;
      protected IMaybe<IObject> _defaultValue;

      public Array(IEnumerable<IObject> objects)
      {
         list = new List<IObject>();
         foreach (var obj in objects)
         {
            if (obj is Range range)
            {
               list.AddRange(range.GetIterator(false).List());
            }
            else
            {
               list.Add(obj);
            }
         }

         arrayID = uniqueObjectID();
         _typeConstraint = none<TypeConstraint>();
         _defaultLambda = none<Lambda>();
         _defaultValue = none<IObject>();
      }

      public Array(IObject value)
      {
         list = new List<IObject> { value };
         arrayID = uniqueObjectID();
         _typeConstraint = none<TypeConstraint>();
         _defaultLambda = none<Lambda>();
         _defaultValue = none<IObject>();
      }

      public string ClassName => "Array";

      public string AsString => list.Select(i => i.AsString).ToString(" ");

      public string Image => $"[{list.Select(i => i.Image).ToString(", ")}]";

      public int Hash => list.GetHashCode();

      public bool IsEqualTo(IObject obj) => isEqualTo(this, obj);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand,
         (a1, a2) =>
         {
            return a1.Length.Value == a2.Length.Value && a1.list.Zip(a2.list, (i1, i2) => i1.Match(i2, bindings)).All(b => b);
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
         get => _typeConstraint;
         set => _typeConstraint = value;
      }

      public IMaybe<Lambda> DefaultLambda
      {
         get => _defaultLambda;
         set => _defaultLambda = value;
      }

      public IMaybe<IObject> DefaultValue
      {
         get => _defaultValue;
         set => _defaultValue = value;
      }

      protected void assertType(IObject value)
      {
         if (_typeConstraint.If(out var tc) && !tc.Matches(classOf(value)))
         {
            throw incompatibleClasses(value, tc.AsString);
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
            else if (_defaultLambda.If(out var lambda))
            {
               return lambda.Invoke(Int.IntObject(index));
            }
            else if (_defaultValue.If(out var value))
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
            if (value is Unit)
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

            return new Array(result);
         }
         set
         {
            switch (value)
            {
               case Array array when array.arrayID == arrayID:
                  return;
               case ICollection collection and not String:
               {
                  var valueIterator = collection.GetIterator(false);
                  foreach (var index in indexList(container, list.Count))
                  {
                     var anyItem = valueIterator.Next();
                     if (anyItem.If(out var item))
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
         if (value is Array array && array.arrayID == arrayID)
         {
            throw "Can't assign an array item to itself".Throws();
         }
      }

      public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

      public IMaybe<IObject> Next(int index) => maybe(index < list.Count, () => this[index]);

      public IMaybe<IObject> Peek(int index) => Next(index);

      public Int Length => list.Count;

      public IEnumerable<IObject> List => list;

      public Slice Slice(ICollection collection) => new(this, collection.GetIterator(false).List().ToArray());

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
         {
            result.AddRange(list);
         }

         return new Array(result);
      }

      public String MakeString(string connector) => makeString(this, connector);

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

      public Boolean IsEmpty => list.Count == 0;

      public IObject Assign(IObject indexes, IObject values)
      {
         if (getIterator(indexes, false).If(out var indexesIterator) && getIterator(values, false).If(out var valuesIterator))
         {
            while (indexesIterator.Next().If(out var index))
            {
               if (valuesIterator.Next().If(out var value))
               {
                  if (index is Int i && i.Value.Between(0).Until(list.Count))
                  {
                     list[i.Value] = value;
                  }
               }
               else
               {
                  break;
               }
            }
         }

         return this;
      }

      public IObject Concatenate(Array array)
      {
         if (_typeConstraint.If(out var thisConstraint))
         {
            if (array._typeConstraint.If(out var otherConstraint))
            {
               if (!thisConstraint.IsEqualTo(otherConstraint))
               {
                  throw "Incompatible type constraints".Throws();
               }
            }
            else
            {
               throw "Expected type constraint in RHS array".Throws();
            }
         }
         else if (array._typeConstraint.IsSome)
         {
            throw "RHS array has a type constraint".Throws();
         }

         var newList = new List<IObject>(list);
         newList.AddRange(array.list);

         return new Array(newList);
      }

      public IObject Pop() => list.Count > 0 ? Some.Object(RemoveAt(list.Count - 1)) : None.NoneValue;

      public IObject Unshift(IObject value) => InsertAt(0, value);

      public IObject Shift() => list.Count > 0 ? Some.Object(RemoveAt(0)) : None.NoneValue;

      public IObject Find(IObject item, int startIndex, bool reverse)
      {
         var index = reverse ? list.LastIndexOf(item, startIndex) : list.IndexOf(item, startIndex);
         return index == -1 ? None.NoneValue : Some.Object((Int)index);
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

      public Array Transpose()
      {
         if (list.All(i => i is Array) && list.Count > 0)
         {
            var listOfLists = list.Select(i => ((Array)i).list.ToArray()).ToArray();
            var minLength = listOfLists.Min(a => a.Length);
            var outerList = new List<IObject>();
            for (var i = 0; i < minLength; i++)
            {
               var innerList = new List<IObject>();
               foreach (var listOf in listOfLists)
               {
                  innerList.Add(listOf[i]);
               }

               outerList.Add(new Array(innerList));
            }

            return new Array(outerList);
         }
         else
         {
            return this;
         }
      }

      public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
   }
}