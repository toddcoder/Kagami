using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Iterators;

public class StreamingIterator(IIterator iterator) : IObject, IIterator
{
   protected IObject collectionAsObject = (IObject)iterator.Collection;
   protected ICollectionClass collectionClass = iterator.CollectionClass;
   protected List<StreamingAction> actions = [];
   protected bool isTerminated;

   public string ClassName => "StreamingIterator";

   public string AsString => $"{collectionAsObject.AsString}.{actions.ToString(".")}";

   public string Image => $"{collectionAsObject.Image}.{actions.ToString(".")}";

   public int Hash => actions.GetHashCode();

   public bool IsEqualTo(IObject obj) => collectionAsObject.IsEqualTo(obj);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => collectionAsObject.IsTrue;

   public Guid Id { get; init; } = Guid.NewGuid();

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");

   public ICollection Collection => iterator.Collection;

   public ICollectionClass CollectionClass => iterator.CollectionClass;

   public bool IsLazy => true;

   public Maybe<IObject> Next()
   {
      if (Machine.Current.Value.Context.Cancelled() || isTerminated)
      {
         return nil;
      }

      var _next = iterator.Next();
      if (_next is (true, var next))
      {
         bool isSkipping;
         do
         {
            isSkipping = false;
            var state = new StreamingState(next, iterator.CollectionClass);
            foreach (var action in actions)
            {
               var condition = action.Execute(state);
               switch (condition)
               {
                  case StreamingCondition.Failed failed:
                     throw fail(failed.Message);
                  case StreamingCondition.Finished:
                     return nil;
                  case StreamingCondition.Continuing iterating:
                     state.Next = iterating.Item;
                     break;
                  case StreamingCondition.Skipping:
                     isSkipping = true;
                     break;
                  case StreamingCondition.Terminated terminated:
                     isTerminated = true;
                     return terminated.Item.Some();
               }

               if (isSkipping)
               {
                  break;
               }
            }

            if (Machine.Current.Value.Context.Cancelled())
            {
               return nil;
            }

            if (isSkipping)
            {
               _next = iterator.Next();
               if (_next is (true, var nextItem))
               {
                  next = nextItem;
               }
               else
               {
                  return nil;
               }
            }
            else
            {
               return state.Next.Some();
            }
         } while (isSkipping);
      }

      return nil;
   }

   public Maybe<IObject> Peek() => iterator.Peek();

   public IObject Reset() => iterator.Reset();

   public IEnumerable<IObject> List()
   {
      while (Next() is (true, var item))
      {
         yield return item;
      }
   }

   public IIterator Clone()
   {
      var streamingIterator = new StreamingIterator(iterator.Clone());
      streamingIterator.actions.AddRange(actions);

      return streamingIterator;
   }

   protected IObject copy(StreamingAction action)
   {
      var streamingIterator = new StreamingIterator(iterator);
      streamingIterator.actions.AddRange(actions);
      streamingIterator.actions.Add(action);

      return streamingIterator;
   }

   protected IIterator terminate() => new KArray(List()).GetIterator(false);

   public IObject Reverse() => terminate().Reverse();

   public KString Join() => terminate().Join();

   public KString Join(string connector) => terminate().Join(connector);

   public KString Join(string connector, int limit, string truncated) => terminate().Join(connector, limit, truncated);

   public IObject Join(Lambda lambda) => terminate().Join(lambda);

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

   public Int Count() => terminate().Count();

   public Int Count(IObject item) => terminate().Count(item);

   public Int Count(Lambda predicate) => terminate().Count(predicate);

   public IObject Map(Lambda lambda) => copy(new StreamingMap(lambda));

   public IObject FlatMap(Lambda lambda) => terminate().FlatMap(lambda);

   public IObject MapAll(Lambda lambda)
   {
      List<IObject> list = [];
      foreach (var item in List())
      {
         if (item is ICollection innerCollection)
         {
            var innerCollectionClass = (ICollectionClass)classOf((IObject)innerCollection);
            var innerIterator = innerCollection.GetIterator(true);
            var mappedItem = innerIterator.List().Select(i => lambda.Invoke(i));
            var newCollection = innerCollectionClass.Revert(mappedItem);
            list.Add(newCollection);
         }
         else
         {
            list.Add(item);
         }
      }

      return collectionClass.Revert(list);
   }

   public IObject Replace(Lambda predicate, Lambda lambda) => terminate().Replace(predicate, lambda);

   public IObject If(Lambda predicate) => copy(new StreamingIf(predicate));

   public IObject IfNot(Lambda predicate) => copy(new StreamingIfNot(predicate));

   public IObject Skip(int count) => copy(new StreamingSkip(count));

   public IObject SkipWhile(Lambda predicate) => copy(new StreamingSkipWhile(predicate));

   public IObject SkipUntil(Lambda predicate) => copy(new StreamingSkipUntil(predicate));

   public IObject Take(int count) => copy(new StreamingTake(count));

   public IObject TakeWhile(Lambda predicate) => copy(new StreamingTakeWhile(predicate));

   public IObject TakeUntil(Lambda predicate) => copy(new StreamingTakeUntil(predicate));

   public IObject Index(Lambda predicate) => terminate().Index(predicate);

   public IObject Indexes(Lambda predicate) => terminate().Indexes(predicate);

   public IObject Zip(ICollection collection) => copy(new StreamingZip(collection));

   public IObject Zip(IIterator zipIterator) => copy(new StreamingZipIterator(zipIterator));

   public IObject Zip(ICollection collection, Lambda lambda) => copy(new StreamingZipLambda(collection, lambda));

   public IObject Zip(IIterator zipIterator, Lambda lambda) => copy(new StreamingZipLambdaIterator(zipIterator, lambda));

   public IObject ZipL(ICollection collection, IObject leftDefaultValue, IObject rightDefaultValue) =>
      terminate().ZipL(collection, leftDefaultValue, rightDefaultValue);

   public IObject ZipL(ICollection collection, IObject leftDefaultValue, IObject rightDefaultValue, Lambda lambda) =>
      terminate().ZipL(collection, leftDefaultValue, lambda);

   public IObject Unzip() => terminate().Unzip();

   public IObject Unzip(Lambda lambda) => terminate().Unzip(lambda);

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

   public IObject GroupBy(Lambda keyLambda, Lambda valueLambda) => terminate().GroupBy(keyLambda, valueLambda);

   public KBoolean One(Lambda predicate) => terminate().One(predicate);

   public KBoolean None(Lambda predicate) => terminate().None(predicate);

   public KBoolean Any(Lambda predicate) => terminate().Any(predicate);

   public KBoolean All(Lambda predicate) => terminate().All(predicate);

   public INumeric Sum() => terminate().Sum();

   public INumeric Average() => terminate().Average();

   public INumeric Product() => terminate().Product();

   public IObject Cross(ICollection collection) => terminate().Cross(collection);

   public IObject Cross(ICollection collection, Lambda lambda) => terminate().Cross(collection, lambda);

   public IObject By(int count) => copy(new StreamingBy(count));

   public IObject Window(int count) => terminate().Window(count);

   public IObject Shape(int rows, int columns) => terminate().Shape(rows, columns);

   public IObject Unique() => copy(new StreamingUnique());

   public IObject Unique(Lambda lambda) => copy(new StreamingUniqueLambda(lambda));

   public IObject Span(Lambda predicate) => terminate().Split(predicate);

   public IObject Span(int count) => terminate().Span(count);

   public IObject Shuffle() => terminate().Shuffle();

   public IObject Random() => terminate().Random();

   public IObject Collect() => terminate().Collect();

   public KArray ToArray() => terminate().ToArray();

   public List ToList() => terminate().ToList();

   public KTuple ToTuple() => terminate().ToTuple();

   public Dictionary ToDictionary(Lambda keyLambda, Lambda valueLambda) => terminate().ToDictionary(keyLambda, valueLambda);

   public IObject ToDictionary() => terminate().ToDictionary();

   public IObject ToSet() => terminate().ToSet();

   public IObject Each(Lambda action) => terminate().Each(action);

   public IObject Rotate(int count) => terminate().Rotate(count);

   public IObject Permutations(int count) => terminate().Permutations(count);

   public IObject Permutations() => terminate().Permutations();

   public IObject Combinations(int count) => terminate().Combinations(count);

   public IObject Combinations() => terminate().Combinations();

   public IObject Flatten() => terminate().Flatten();

   public IObject Copy() => terminate().Copy();

   public IObject Apply(ICollection collection) => terminate().Apply(collection);

   public IObject Column(int column) => terminate().Column(column);

   public IObject Partition(Lambda lambda) => terminate().Partition(lambda);

   public IObject Pick(int count) => terminate().Pick(count);

   public IObject Pick() => terminate().Pick();

   public IObject Roll(int count) => terminate().Roll(count);

   public IObject Splat(int count) => terminate().Splat(count);

   public IObject Chunked(int count) => terminate().Chunked(count);

   public IObject Windowed(int size, int step, bool partial) => terminate().Windowed(size, step, partial);

   public IObject Repeated() => terminate().Repeated();

   public IObject Accumulate(Lambda lambda) => terminate().Last(lambda);

   public IObject Accumulate(IObject initialValue, Lambda lambda) => terminate().Accumulate(initialValue, lambda);

   public KBoolean AllTrue(IObject argument) => terminate().AllTrue(argument);

   public KBoolean AnyTrue(IObject argument) => terminate().AnyTrue(argument);

   public KBoolean NoneTrue(IObject argument) => terminate().NoneTrue(argument);

   public KTuple HeadTail() => terminate().HeadTail();

   public Junction JunctionAll() => terminate().JunctionAll();

   public Junction JunctionAny() => terminate().JunctionAny();

   public Junction JunctionNone() => terminate().JunctionNone();

   public Junction JunctionOne() => terminate().JunctionOne();

   public IObject Step(int step) => terminate().Step(step);

   public IObject this[int index]
   {
      get
      {
         if (index > -1)
         {
            Maybe<IObject> _next = nil;
            for (var i = 0; i < index; i++)
            {
               _next = Next();
            }

            return _next.Required("Iterator index out of bounds");
         }
         else
         {
            var list = List().ToList();
            index = wrapIndex(index, list.Count);
            return list[index];
         }
      }
   }

   public Sequence Seq() => terminate().Seq();

   public IObject Transpose() => terminate().Transpose();

   public IObject Assoc(IObject target) => copy(new StreamingAssoc(target));

   public TypeConstraint EquivalentTypeConstraint() => Objects.TypeConstraint.FromList("Iterator");
}