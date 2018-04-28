using System;
using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Runtime;
using Standard.Types.Arrays;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Objects
{
   public struct FlexRange : IObject, ICollection
   {
      IObject from;
      Lambda by;
      IMaybe<IObjectCompare> to;
      bool inclusive;
      Func<IObject, bool> compare;

      public FlexRange(IObject from, Lambda by) : this()
      {
         this.from = from;
         this.by = by;
         to = none<IObjectCompare>();

      }

      public FlexRange(IObject from, Lambda by, IObjectCompare to, bool inclusive) : this()
      {
         this.from = from;
         this.by = by;
         this.to = to.Some();
         this.inclusive = inclusive;
         var self = this;
         if (this.to.If(out var oc) && self.to.If(out var selfTo))
            if (oc.Compare(from) >= 0)
            {
               if (inclusive)
                  compare = c => selfTo.Compare(c) >= 0;
               else
                  compare = c => selfTo.Compare(c) > 0;
            }
            else
            {
               if (inclusive)
                  compare = c => selfTo.Compare(c) <= 0;
               else
                  compare = c => selfTo.Compare(c) < 0;
            }
         else
            compare = c => true;
      }

      public IObject From => from;

      public Lambda By => by;

      public IObject To => to.FlatMap(oc => new Some((IObject)oc), () => Nil.NilValue);

      public Boolean Inclusive => inclusive;

      public string ClassName => "FlexRange";

      public string AsString => $"from {from.Image} by {by.Image}{to.FlatMap(t => $" to {((IObject)t).Image}", () => "")}";

      public string Image => AsString;

      public int Hash => (from.Hash + by.Hash + to.FlatMap(t => ((IObject)t).Hash, () => 0)).GetHashCode();

      static bool isEqual(IMaybe<IObjectCompare> left, IMaybe<IObjectCompare> right)
      {
         if (left.IsNone && right.IsNone)
            return true;
         else if (left.If(out var l) && right.If(out var r))
            return ((IObject)l).IsEqualTo((IObject)r);
         else
            return false;
      }

      public bool IsEqualTo(IObject obj)
      {
         return obj is FlexRange fr && from.IsEqualTo(fr.from) && by.IsEqualTo(fr.by) && isEqual(to, fr.to) &&
            inclusive == fr.inclusive;
      }

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => true;

      public IIterator GetIterator(bool lazy) => new CollectionProxy(this).GetIterator(lazy);

      public IMaybe<IObject> Next(int index) => none<IObject>();

      public IMaybe<IObject> Peek(int index) => none<IObject>();

      public Int Length
      {
         get
         {
            var self = this;
            return to.FlatMap(t => self.List.Count(), () => throw collectionInfinite());
         }
      }

      public IEnumerable<IObject> List
      {
         get
         {
            if (compare is null)
               yield break;

            var current = from;
            while (compare(current))
            {
               yield return current;

               current = by.Invoke(current);

               if (Machine.Current.Context.Cancelled())
                  yield break;
            }
         }
      }

      public bool ExpandForArray => true;

      public Boolean In(IObject item)
      {
         var self = this;
         return to.FlatMap(t => self.List.ToArray().Contains(item), () => throw collectionInfinite());
      }

      public Boolean NotIn(IObject item)
      {
         var self = this;
         return to.FlatMap(t => !self.List.ToArray().Contains(item), () => throw collectionInfinite());
      }

      public IObject Times(int count)
      {
         var self = this;
         return to.FlatMap(t => new Array(self.List.ToArray().Repeat(count)), () => throw collectionInfinite());
      }
   }
}