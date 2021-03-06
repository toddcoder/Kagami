﻿using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
   public class CycleCollection : IObject, ICollection
   {
      protected Cycle cycle;
      protected IMaybe<IObject> _lastItem;
      protected IMaybe<(IObject, Lambda)> _seedLambda;

      public CycleCollection(Cycle cycle)
      {
         this.cycle = cycle;
         _lastItem = none<IObject>();
         _seedLambda = cycle.SeedLambda;
      }

      public string ClassName => cycle.ClassName;

      public string AsString => cycle.AsString;

      public string Image => cycle.Image;

      public int Hash => cycle.Hash;

      public bool IsEqualTo(IObject obj) => cycle.IsEqualTo(obj);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => cycle.Match(comparisand, bindings);

      public bool IsTrue => cycle.IsTrue;

      public IIterator GetIterator(bool lazy) => new LazyIterator(this);

      public IMaybe<IObject> Next(int index)
      {
         if (_seedLambda.If(out var seedLambda))
         {
            var (seed, lambda) = seedLambda;
            if (_lastItem.If(out var lastItem))
            {
               lastItem = lambda.Invoke(lastItem);
               _lastItem = lastItem.Some();
            }
            else
            {
               _lastItem = seed.Some();
            }
         }
         else
         {
            var item = cycle[index++ % cycle.Length.Value];
            if (item is Lambda lambda)
            {
               var parameterCount = lambda.Invokable.Parameters.Length;
               if (parameterCount > 0 && _lastItem.If(out var lastItem))
               {
                  item = lambda.Invoke(lastItem);
               }
               else
               {
                  item = lambda.Invoke();
               }

               _lastItem = item.Some();
            }
            else
            {
               _lastItem = item.Some();
            }
         }

         return _lastItem;
      }

      public IMaybe<IObject> Peek(int index) => _lastItem;

      public Int Length => cycle.Length;

      public bool ExpandForArray => cycle.ExpandForArray;

      public Boolean In(IObject item) => cycle.In(item);

      public Boolean NotIn(IObject item) => cycle.NotIn(item);

      public IObject Times(int count) => cycle.Times(count);

      public String MakeString(string connector) => cycle.MakeString(connector);

      public IIterator GetIndexedIterator() => new IndexedIterator(this);

      public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
   }
}