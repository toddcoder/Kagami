﻿using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Classes;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Objects
{
   public class Slice : IObject, ICollection
   {
      IObject original;
      ISliceable sliceable;
      IObject[] indexes;
      ICollectionClass collectionClass;

      public Slice(ISliceable sliceable, IObject[] indexes)
      {
         this.sliceable = sliceable;
         this.indexes = indexes;
         original = (IObject)sliceable;
         collectionClass = (ICollectionClass)classOf(original);
      }

      public string ClassName => "Slice";

      public IObject Reverted() => collectionClass.Revert(List);

      public string AsString => collectionClass.Revert(List.Select(i => (IObject)(String)i.AsString)).AsString;

      public string Image => collectionClass.Revert(List.Select(i => (IObject)(String)i.Image)).AsString;

      public int Hash => List.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Slice slice && AsString == slice.AsString;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => List.Any();

      public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

      public IMaybe<IObject> Next(int index) => when(index < indexes.Length, () => sliceable.Get(indexes[index]));

      public IMaybe<IObject> Peek(int index) => Next(index);

      public Int Length => indexes.Length;

      public IEnumerable<IObject> List
      {
         get
         {
            foreach (var index in indexes)
               if (sliceable.Get(index).If(out var value))
                  yield return value;
               else
                  yield break;
         }
      }

      public bool ExpandForArray => sliceable.ExpandForArray;

      public Boolean In(IObject item) => indexes.Select(i => sliceable.Get(i).FlatMap(o => o.IsEqualTo(item), () => false)).Any();

      public Boolean NotIn(IObject item)
      {
         return indexes.Select(i => sliceable.Get(i).FlatMap(o => !o.IsEqualTo(item), () => false)).All(b => b);
      }

      public IObject Times(int count) => ((ICollection)Reverted()).Times(count);

      public IObject Assign(IObject value)
      {
         if (value is ICollection collection)
         {
            var target = List.ToArray();
            var source = collection.GetIterator(false).List();
            target = setObjects(target, source, i => Unassigned.Value);

            for (var i = 0; i < target.Length; i++)
            {
               var index = indexes[i];
               sliceable.Set(index, target[i]);
            }
         }
         else
            foreach (var index in indexes)
               sliceable.Set(index, value);

         return this;
      }
   }
}