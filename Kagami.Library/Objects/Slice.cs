using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Classes;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Objects;

public class Slice : IObject, ICollection
{
   protected ISliceable sliceable;
   protected IObject[] indexes;
   protected ICollectionClass collectionClass;

   public Slice(ISliceable sliceable, IObject[] indexes)
   {
      this.sliceable = sliceable;
      this.indexes = indexes;
      var original = (IObject)sliceable;
      collectionClass = (ICollectionClass)classOf(original);
   }

   public string ClassName => "Slice";

   public IObject Reverted() => collectionClass.Revert(List);

   public string AsString => collectionClass.Revert(List.Select(i => (IObject)(KString)i.AsString)).AsString;

   public string Image => collectionClass.Revert(List.Select(i => (IObject)(KString)i.Image)).AsString;

   public int Hash => List.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Slice slice && AsString == slice.AsString;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => List.Any();

   public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

   public Maybe<IObject> Next(int index) => maybe<IObject>() & index < indexes.Length & (() => sliceable.Get(indexes[index]));

   public Maybe<IObject> Peek(int index) => Next(index);

   public Int Length => indexes.Length;

   public IEnumerable<IObject> List
   {
      get
      {
         foreach (var index in indexes)
         {
            var _value = sliceable.Get(index);
            if (_value is (true, var value))
            {
               yield return value;
            }
            else
            {
               yield break;
            }
         }
      }
   }

   public bool ExpandForArray => sliceable.ExpandForArray;

   public KBoolean In(IObject item) => indexes.Select(i => sliceable.Get(i).Map(o => o.IsEqualTo(item)) | false).Any();

   public KBoolean NotIn(IObject item)
   {
      return indexes.Select(i => sliceable.Get(i).Map(o => !o.IsEqualTo(item)) | false).All(b => b);
   }

   public IObject Times(int count) => ((ICollection)Reverted()).Times(count);

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject Assign(IObject value)
   {
      if (value is ICollection collection)
      {
         var target = List.ToArray();
         var source = collection.GetIterator(false).List();
         target = setObjects(target, source, _ => Unassigned.Value);

         for (var i = 0; i < target.Length; i++)
         {
            var index = indexes[i];
            sliceable.Set(index, target[i]);
         }
      }
      else
      {
         foreach (var index in indexes)
         {
            sliceable.Set(index, value);
         }
      }

      return this;
   }

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
}