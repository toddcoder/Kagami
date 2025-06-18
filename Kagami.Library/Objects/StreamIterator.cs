using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class StreamIterator : IObject, IIterator
{
   protected IIterator iterator;
   protected List<IStreamAction> actions = [];

   public StreamIterator(IIterator iterator)
   {
      this.iterator = iterator.Clone();
   }

   public string ClassName => "StreamIterator";

   public string AsString => actions.ToString(" ");

   public string Image => AsString;

   public int Hash => actions.GetHashCode();

   public bool IsEqualTo(IObject obj) => false;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => false;

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public ICollection Collection => iterator.Collection;

   public ICollectionClass CollectionClass => iterator.CollectionClass;

   public bool IsLazy => true;

   protected IEnumerable<IObject> list()
   {
      Maybe<IObject> _item;
      do
      {
         _item = Next();
         if (_item is (true, var item))
         {
            yield return item;
         }
      } while (_item);
   }

   public Maybe<IObject> Next()
   {
      while (!Machine.Current.Value.Context.Cancelled())
      {
         var _next = iterator.Next();
         if (_next is (true, var next))
         {
            var status = Accepted.New(next);

            foreach (var action in actions)
            {
               status = action.Next(status);
               if (status.IsSkipped)
               {
                  break;
               }
               else if (status.IsEnded)
               {
                  return nil;
               }
            }

            if (status.IsAccepted)
            {
               return status.Object.Some();
            }
            else if (status.IsEnded)
            {
               return nil;
            }
         }
         else
         {
            return nil;
         }
      }

      return nil;
   }

   public Maybe<IObject> Peek() => Next();

   public IObject Reset()
   {
      terminate().Reset();
      return this;
   }

   public IEnumerable<IObject> List() => list();

   public IIterator Clone()
   {
      var newIterator = new StreamIterator(iterator);
      newIterator.actions.AddRange(actions);

      return newIterator;
   }

   public IIterator terminate()
   {
      var list = List().ToList();
      var array = new KArray(list);

      return array.GetIterator(false);
   }

   public IObject Reverse() => terminate().Reverse();

   public KString Join(string connector) => terminate().Join(connector);

   public IObject Sort(Lambda lambda, bool ascending) => terminate().Sort(lambda, ascending);

   public IObject Sort(bool ascending) => terminate().Sort(ascending);

   public IObject FoldLeft(IObject initialValue, Lambda lambda) => terminate().FoldLeft(initialValue, lambda);

   public IObject FoldLeft(Lambda lambda) => terminate().FoldLeft(lambda);

   public IObject FoldRight(IObject initialValue, Lambda lambda) => terminate().FoldRight(initialValue, lambda);

   public IObject FoldRight(Lambda lambda) => terminate().FoldRight(lambda);

   public IObject ReduceLeft(IObject initialValue, Lambda lambda) => terminate().ReduceLeft(initialValue, lambda);

   public IObject ReduceLeft(Lambda lambda) => terminate().ReduceLeft(lambda);

   public IObject ReduceRight(IObject initialValue, Lambda lambda) => terminate().ReduceRight(initialValue, lambda);

   public IObject ReduceRight(Lambda lambda) => terminate().ReduceRight(lambda);

   public Int Count(IObject item) => terminate().Count(item);

   public Int Count(Lambda predicate) => terminate().Count(predicate);

   public IObject Copy(IStreamAction action)
   {
      var newIterator = new StreamIterator(iterator);
      newIterator.actions.AddRange(actions);
      newIterator.actions.Add(action);

      return newIterator;
   }

   public IObject Map(Lambda lambda) => Copy(new MapAction(lambda));

   public IObject FlatMap(Lambda lambda) => Copy(new FlatMapAction(lambda));

   public IObject Replace(Lambda predicate, Lambda lambda) => Copy(new MapIfAction(lambda, predicate));

   public IObject If(Lambda predicate) => Copy(new IfAction(predicate));

   public IObject IfNot(Lambda predicate) => Copy(new IfNotAction(predicate));

   protected static IStreamAction getSkipAction(ICollection collection, int count)
   {
      if (count > -1)
      {
         return new SkipAction(count);
      }
      else
      {
         var length = collection.Length.Value;
         if (length == -1)
         {
            return new SkipAction(-count);
         }
         else
         {
            return new TakeAction(length + count);
         }
      }
   }

   public IObject Skip(int count) => Copy(getSkipAction(Collection, count));

   public IObject SkipWhile(Lambda predicate) => Copy(new SkipWhileAction(predicate));

   public IObject SkipUntil(Lambda predicate) => Copy(new SkipUntilAction(predicate));

   protected static IStreamAction getTakeAction(ICollection collection, int count)
   {
      if (count > -1)
      {
         return new TakeAction(count);
      }
      else
      {
         var length = collection.Length.Value;
         if (length == -1)
         {
            return new TakeAction(-count);
         }
         else
         {
            return new SkipAction(length + count);
         }
      }
   }

   public IObject Take(int count) => Copy(getTakeAction(Collection, count));

   public IObject TakeWhile(Lambda predicate) => Copy(new TakeWhileAction(predicate));

   public IObject TakeUntil(Lambda predicate) => Copy(new TakeUntilAction(predicate));

   public IObject Index(Lambda predicate) => terminate().Index(predicate);

   public IObject Indexes(Lambda predicate) => terminate().Indexes(predicate);

   public IObject Zip(ICollection collection) => terminate().Zip(collection);

   public IObject Zip(ICollection collection, Lambda lambda) => terminate().Zip(collection, lambda);

   public IObject Min() => terminate().Min();

   public IObject Min(Lambda lambda) => terminate().Min(lambda);

   public IObject Max() => terminate().Max();

   public IObject Max(Lambda lambda) => terminate().Max(lambda);

   public IObject First() => terminate().First();

   public IObject First(Lambda predicate) => terminate().First(predicate);

   public IObject Last() => terminate().Last();

   public IObject Last(Lambda predicate) => terminate().Last(predicate);

   public IObject Split(Lambda predicate) => terminate().Split(predicate);

   public IObject Split(int count) => terminate().Split(count);

   public IObject GroupBy(Lambda lambda) => terminate().GroupBy(lambda);

   public KBoolean One(Lambda predicate) => terminate().One(predicate);

   public KBoolean None(Lambda predicate) => terminate().None(predicate);

   public KBoolean Any(Lambda predicate) => terminate().Any(predicate);

   public KBoolean All(Lambda predicate) => terminate().All(predicate);

   public INumeric Sum() => terminate().Sum();

   public INumeric Average() => terminate().Average();

   public INumeric Product() => terminate().Product();

   public IObject Cross(ICollection collection) => terminate().Cross(collection);

   public IObject Cross(ICollection collection, Lambda lambda) => terminate().Cross(collection, lambda);

   public IObject By(int count) => terminate().By(count);

   public IObject Window(int count) => terminate().Window(count);

   public IObject Shape(int rows, int columns) => terminate().Shape(rows, columns);

   public IObject Distinct() => Copy(new DistinctAction());

   public IObject Span(Lambda predicate) => terminate().Span(predicate);

   public IObject Span(int count) => terminate().Span(count);

   public IObject Shuffle() => terminate().Shuffle();

   public IObject Random() => terminate().Random();

   public IObject Collect() => terminate().Collect();

   public KArray ToArray() => new(List());

   public List ToList() => Objects.List.NewList(List());

   public KTuple ToTuple() => new(List().ToArray());

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

   public IObject ToDictionary() => KArray.CreateObject(List());

   public IObject ToSet() => new Set(List().ToArray());

   public IObject Each(Lambda action)
   {
      foreach (var item in List())
      {
         action.Invoke(item);
      }

      return this;
   }

   public IObject Rotate(int count) => terminate().Rotate(count);

   public IObject Permutation(int count) => terminate().Permutation(count);

   public IObject Combination(int count) => terminate().Combination(count);

   public IObject Flatten() => terminate().Flatten();

   public IObject Copy() => terminate().Copy();

   public IObject Apply(ICollection collection) => terminate().Apply(collection);

   public IObject Column(int column) => terminate().Column(column);

   public IObject Partition(Lambda lambda) => terminate().Partition(lambda);

   public BaseClass Equivalent() => new CollectionClass();

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");
}