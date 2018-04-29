using System.Collections.Generic;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Objects
{
   public class StreamIterator : IObject, IIterator
   {
      IIterator iterator;
      List<IStreamAction> actions;

      public StreamIterator(IIterator iterator)
      {
         this.iterator = iterator;
         actions = new List<IStreamAction>();
      }

      public StreamIterator(IIterator iterator, IStreamAction action) : this(iterator) => actions.Add(action);

      public string ClassName => "StreamIterator";

      public string AsString => "";

      public string Image => "";

      public int Hash => 0;

      public bool IsEqualTo(IObject obj) => false;

	   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => false;

	   public bool IsTrue => true;

      public ICollection Collection => iterator.Collection;

      public bool IsLazy => true;

      IEnumerable<IObject> list()
      {
         var item = none<IObject>();
         do
         {
            item = iterator.Next();
            if (item.If(out var obj))
               yield return obj;
         } while (item.IsSome);
      }

      public IMaybe<IObject> Next()
      {
         foreach (var value in list())
         {
            var status = Accepted.New(value);

            foreach (var action in actions)
            {
               status = action.Next(status);
               if (status.IsSkipped)
                  continue;

               if (status.IsEnded)
                  return none<IObject>();

               if (status.IsAccepted)
                  return status.Object.Some();
            }
         }

         return none<IObject>();
      }

      public IMaybe<IObject> Peek() => Next();

      public IEnumerable<IObject> List() => list();

      public IObject Reverse() => iterator.Reverse();

      public String Join(string connector) => iterator.Join(connector);

      public IObject Sort(Lambda lambda, bool ascending) => iterator.Sort(lambda, @ascending);

      public IObject Sort(bool ascending) => iterator.Sort(@ascending);

      public IObject FoldLeft(IObject initialValue, Lambda lambda) => iterator.FoldLeft(initialValue, lambda);

      public IObject FoldLeft(Lambda lambda) => iterator.FoldLeft(lambda);

      public IObject FoldRight(IObject initialValue, Lambda lambda) => iterator.FoldRight(initialValue, lambda);

      public IObject FoldRight(Lambda lambda) => iterator.FoldRight(lambda);

      public IObject ReduceLeft(IObject initialValue, Lambda lambda) => iterator.ReduceLeft(initialValue, lambda);

      public IObject ReduceLeft(Lambda lambda) => iterator.ReduceLeft(lambda);

      public IObject ReduceRight(IObject initialValue, Lambda lambda) => iterator.ReduceRight(initialValue, lambda);

      public IObject ReduceRight(Lambda lambda) => iterator.ReduceRight(lambda);

      public Int Count(Lambda predicate) => iterator.Count(predicate);

      public IObject Copy(IStreamAction action)
      {
         var newIterator = new StreamIterator(iterator);
         newIterator.actions.AddRange(actions);
         newIterator.actions.Add(action);

         return newIterator;
      }

      public IObject Map(Lambda lambda) => Copy(new MapAction(lambda));

      public IObject If(Lambda predicate) => Copy(new IfAction(predicate));

      public IObject IfNot(Lambda predicate) => Copy(new IfNotAction(predicate));

      static IStreamAction getSkipAction(ICollection collection, int count)
      {
         if (count > -1)
            return new SkipAction(count);

         var length = collection.Length.Value;
         if (length == -1)
            return new SkipAction(-count);

         return new TakeAction(length + count);
      }

      public IObject Skip(int count) => Copy(getSkipAction(Collection, count));

      public IObject SkipWhile(Lambda predicate) => Copy(new SkipWhileAction(predicate));

      public IObject SkipUntil(Lambda predicate) => Copy(new SkipUntilAction(predicate));

      static IStreamAction getTakeAction(ICollection collection, int count)
      {
         if (count > -1)
            return new TakeAction(count);

         var length = collection.Length.Value;
         if (length == -1)
            return new TakeAction(-count);

         return new SkipAction(length + count);
      }

      public IObject Take(int count) => Copy(getTakeAction(Collection, count));

      public IObject TakeWhile(Lambda predicate) => Copy(new TakeWhileAction(predicate));

      public IObject TakeUntil(Lambda predicate) => Copy(new TakeUntilAction(predicate));

      public IObject Index(Lambda predicate) => iterator.Index(predicate);

      public IObject Indexes(Lambda predicate) => iterator.Indexes(predicate);

      public IObject Zip(ICollection collection) => iterator.Zip(collection);

      public IObject Zip(ICollection collection, Lambda lambda) => iterator.Zip(collection, lambda);

      public IObject Min() => iterator.Min();

      public IObject Max() => iterator.Max();

      public IObject First() => iterator.First();

      public IObject First(Lambda predicate) => iterator.First(predicate);

      public IObject Last() => iterator.Last();

      public IObject Last(Lambda predicate) => iterator.Last(predicate);

      public IObject Split(Lambda predicate) => iterator.Split(predicate);

      public IObject Split(int count) => iterator.Split(count);

      public IObject Group(Lambda lambda) => iterator.Group(lambda);

      public Boolean One(Lambda predicate) => iterator.One(predicate);

      public Boolean None(Lambda predicate) => iterator.None(predicate);

      public Boolean Any(Lambda predicate) => iterator.Any(predicate);

      public Boolean All(Lambda predicate) => iterator.All(predicate);

      public INumeric Sum() => iterator.Sum();

      public INumeric Average() => iterator.Average();

      public INumeric Product() => iterator.Product();

      public IObject Cross(ICollection collection) => iterator.Cross(collection);

      public IObject By(int count) => iterator.By(count);

      public IObject Distinct() => Copy(new DistinctAction());

      public IObject Span(Lambda predicate) => iterator.Span(predicate);

      public IObject Span(int count) => iterator.Span(count);
   }
}