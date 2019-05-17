using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
   public class StreamIterator : IObject, IIterator
   {
      IIterator iterator;
      List<IStreamAction> actions;

      public StreamIterator(IIterator iterator)
      {
	      this.iterator = iterator.Clone();
         actions = new List<IStreamAction>();
      }

      public string ClassName => "StreamIterator";

      public string AsString => actions.Join(" ");

      public string Image => AsString;

      public int Hash => actions.GetHashCode();

      public bool IsEqualTo(IObject obj) => false;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => false;

      public bool IsTrue { get; } = true;

      public ICollection Collection => iterator.Collection;

      public ICollectionClass CollectionClass => iterator.CollectionClass;

      public bool IsLazy => true;

      IEnumerable<IObject> list()
      {
         var item = none<IObject>();
         do
         {
            item = Next();
            if (item.If(out var obj))
               yield return obj;
         } while (item.IsSome);
      }

      public IMaybe<IObject> Next()
      {
         while (!Machine.Current.Context.Cancelled())
            if (iterator.Next().If(out var value))
            {
               var status = Accepted.New(value);

               foreach (var action in actions)
               {
                  status = action.Next(status);
                  if (status.IsSkipped)
                     break;
                  else if (status.IsEnded)
                     return none<IObject>();
               }

               if (status.IsAccepted)
                  return status.Object.Some();
               else if (status.IsEnded)
                  return none<IObject>();
            }
            else
               return none<IObject>();

         return none<IObject>();
      }

      public IMaybe<IObject> Peek() => Next();

      public IObject Reset()
      {
	      iterator.Reset();
	      return this;
      }

      public IEnumerable<IObject> List() => list();

      public IIterator Clone()
      {
	      var newIterator = new StreamIterator(iterator);
	      newIterator.actions.AddRange(actions);

	      return newIterator;
        }

      public IObject Reverse() => iterator.Reverse();

      public String Join(string connector) => iterator.Join(connector);

      public IObject Sort(Lambda lambda, bool ascending) => iterator.Sort(lambda, ascending);

      public IObject Sort(bool ascending) => iterator.Sort(ascending);

      public IObject FoldLeft(IObject initialValue, Lambda lambda) => iterator.FoldLeft(initialValue, lambda);

      public IObject FoldLeft(Lambda lambda) => iterator.FoldLeft(lambda);

      public IObject FoldRight(IObject initialValue, Lambda lambda) => iterator.FoldRight(initialValue, lambda);

      public IObject FoldRight(Lambda lambda) => iterator.FoldRight(lambda);

      public IObject ReduceLeft(IObject initialValue, Lambda lambda) => iterator.ReduceLeft(initialValue, lambda);

      public IObject ReduceLeft(Lambda lambda) => iterator.ReduceLeft(lambda);

      public IObject ReduceRight(IObject initialValue, Lambda lambda) => iterator.ReduceRight(initialValue, lambda);

      public IObject ReduceRight(Lambda lambda) => iterator.ReduceRight(lambda);

      public Int Count(IObject item) => iterator.Count(item);

      public Int Count(Lambda predicate) => iterator.Count(predicate);

      public IObject Copy(IStreamAction action)
      {
         var newIterator = new StreamIterator(iterator);
         newIterator.actions.AddRange(actions);
         newIterator.actions.Add(action);

         return newIterator;
      }

      public IObject Map(Lambda lambda) => Copy(new MapAction(lambda));

      public IObject FlatMap(Lambda lambda) => Copy(new FlatMapAction(lambda));

      public IObject If(Lambda predicate) => Copy(new IfAction(predicate));

      public IObject IfNot(Lambda predicate) => Copy(new IfNotAction(predicate));

      static IStreamAction getSkipAction(ICollection collection, int count)
      {
         if (count > -1)
            return new SkipAction(count);
         else
         {
            var length = collection.Length.Value;
            if (length == -1)
               return new SkipAction(-count);
            else
               return new TakeAction(length + count);
         }
      }

      public IObject Skip(int count) => Copy(getSkipAction(Collection, count));

      public IObject SkipWhile(Lambda predicate) => Copy(new SkipWhileAction(predicate));

      public IObject SkipUntil(Lambda predicate) => Copy(new SkipUntilAction(predicate));

      static IStreamAction getTakeAction(ICollection collection, int count)
      {
         if (count > -1)
            return new TakeAction(count);
         else
         {
            var length = collection.Length.Value;
            if (length == -1)
               return new TakeAction(-count);
            else
               return new SkipAction(length + count);
         }
      }

      public IObject Take(int count) => Copy(getTakeAction(Collection, count));

      public IObject TakeWhile(Lambda predicate) => Copy(new TakeWhileAction(predicate));

      public IObject TakeUntil(Lambda predicate) => Copy(new TakeUntilAction(predicate));

      public IObject Index(Lambda predicate) => iterator.Index(predicate);

      public IObject Indexes(Lambda predicate) => iterator.Indexes(predicate);

      public IObject Zip(ICollection collection) => iterator.Zip(collection);

      public IObject Zip(ICollection collection, Lambda lambda) => iterator.Zip(collection, lambda);

      public IObject Min() => iterator.Min();

      public IObject Min(Lambda lambda) => iterator.Min(lambda);

      public IObject Max() => iterator.Max();

      public IObject Max(Lambda lambda) => iterator.Max(lambda);

      public IObject First() => iterator.First();

      public IObject First(Lambda predicate) => iterator.First(predicate);

      public IObject Last() => iterator.Last();

      public IObject Last(Lambda predicate) => iterator.Last(predicate);

      public IObject Split(Lambda predicate) => iterator.Split(predicate);

      public IObject Split(int count) => iterator.Split(count);

      public IObject GroupBy(Lambda lambda) => iterator.GroupBy(lambda);

      public Boolean One(Lambda predicate) => iterator.One(predicate);

      public Boolean None(Lambda predicate) => iterator.None(predicate);

      public Boolean Any(Lambda predicate) => iterator.Any(predicate);

      public Boolean All(Lambda predicate) => iterator.All(predicate);

      public INumeric Sum() => iterator.Sum();

      public INumeric Average() => iterator.Average();

      public INumeric Product() => iterator.Product();

      public IObject Cross(ICollection collection) => iterator.Cross(collection);

      public IObject By(int count) => iterator.By(count);

      public IObject Window(int count) => iterator.Window(count);

      public IObject Distinct() => Copy(new DistinctAction());

      public IObject Span(Lambda predicate) => iterator.Span(predicate);

      public IObject Span(int count) => iterator.Span(count);

      public IObject Shuffle() => iterator.Shuffle();

      public IObject Random() => iterator.Random();

      public IObject Collect() => iterator.Collect();

      public Array ToArray() => new Array(List());

      public List ToList() => Objects.List.NewList(List());

      public Tuple ToTuple() => new Tuple(List().ToArray());

      public Dictionary ToDictionary(Lambda keyLambda, Lambda valueLambda)
      {
         var hash = new Hash<IObject, IObject>();

         foreach (var item in List())
         {
            var key = keyLambda.Invoke(item);
            var value = valueLambda.Invoke(item);
            hash[key] = value;
         }

         return new Dictionary(hash);
      }

      public IObject ToDictionary() => Array.CreateObject(List());

      public IObject Each(Lambda action)
      {
         foreach (var item in List())
            action.Invoke(item);
         return this;
      }

      public IObject Rotate(int count) => iterator.Rotate(count);

      public IObject Permutation(int count) => iterator.Permutation(count);

      public IObject Combination(int count) => iterator.Combination(count);

      public IObject Flatten() => iterator.Flatten();

      public IObject Copy() => iterator.Copy();

      public IObject Apply(ICollection collection) => iterator.Apply(collection);

      public BaseClass Equivalent() => new CollectionClass();

      public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");
   }
}