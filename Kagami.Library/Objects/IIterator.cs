using Kagami.Library.Classes;
using Core.Monads;

namespace Kagami.Library.Objects;

public interface IIterator : IEquivalentClass
{
   ICollection Collection { get; }

   ICollectionClass CollectionClass { get; }

   bool IsLazy { get; }

   Maybe<IObject> Next();

   Maybe<IObject> Peek();

   IObject Reset();

   IEnumerable<IObject> List();

   IIterator Clone();

   IObject Reverse();

   KString Join();

   KString Join(string connector);

   KString Join(string connector, int limit, string truncated);

   IObject Join(Lambda lambda);

   KString Join(string connector, string prefix, string suffix);

   IObject Sort(Lambda lambda, bool ascending);

   IObject Sort(bool ascending);

   IObject FoldLeft(IObject initialValue, Lambda lambda);

   IObject FoldLeft(Lambda lambda);

   IObject FoldRight(IObject initialValue, Lambda lambda);

   IObject FoldRight(Lambda lambda);

   IObject ReduceLeft(IObject initialValue, Lambda lambda);

   IObject ReduceLeft(Lambda lambda);

   IObject ReduceRight(IObject initialValue, Lambda lambda);

   IObject ReduceRight(Lambda lambda);

   Int Count();

   Int Count(IObject item);

   Int Count(Lambda predicate);

   IObject Map(Lambda lambda);

   IObject FlatMap(Lambda lambda);

   IObject MapAll(Lambda lambda);

   IObject MapIf(Lambda lambda);

   IObject Replace(Lambda predicate, Lambda lambda);

   IObject If(Lambda predicate);

   IObject IfNot(Lambda predicate);

   IObject Skip(int count);

   IObject SkipWhile(Lambda predicate, bool back);

   IObject SkipUntil(Lambda predicate, bool back);

   IObject Take(int count);

   IObject TakeWhile(Lambda predicate, bool back);

   IObject TakeUntil(Lambda predicate, bool back);

   IObject Index(Lambda predicate);

   IObject Indexes(Lambda predicate);

   IObject Zip(ICollection collection);

   IObject Zip(IIterator iterator);

   IObject Zip(OpenRange openRange);

   IObject Zip(NumericOpenRange openRange);

   IObject Zip(ICollection collection, Lambda lambda);

   IObject Zip(IIterator iterator, Lambda lambda);

   IObject Zip(OpenRange openRange, Lambda lambda);

   IObject Zip(NumericOpenRange openRange, Lambda lambda);

   IObject ZipL(ICollection collection, IObject leftDefaultValue, IObject rightDefaultValue);

   IObject ZipL(ICollection collection, IObject leftDefaultValue, IObject rightDefaultValue, Lambda lambda);

   IObject Unzip();

   IObject Unzip(Lambda lambda);

   IObject Min();

   IObject Min(Lambda lambda);

   IObject Max();

   IObject Max(Lambda lambda);

   IObject First();

   IObject First(Lambda predicate);

   IObject Last();

   IObject Last(Lambda predicate);

   IObject Split(Lambda predicate);

   IObject Split(int count);

   IObject GroupBy(Lambda lambda);

   IObject GroupBy(Lambda keyLambda, Lambda valueLambda);

   KBoolean One(Lambda predicate);

   KBoolean None(Lambda predicate);

   KBoolean Any(Lambda predicate);

   KBoolean All(Lambda predicate);

   INumeric Sum();

   INumeric Average();

   INumeric Product();

   IObject Cross(ICollection collection);

   IObject Cross(ICollection collection, Lambda lambda);

   IObject By(int count);

   IObject Window(int count);

   IObject Shape(int rows, int columns);

   IObject Unique();

   IObject Unique(Lambda lambda);

   IObject Span(Lambda predicate);

   IObject Span(int count);

   IObject Shuffle();

   IObject Random();

   IObject Collect();

   KArray ToArray();

   List ToList();

   KTuple ToTuple();

   Dictionary ToDictionary(Lambda keyLambda, Lambda valueLambda);

   IObject ToDictionary();

   IObject ToSet();

   IObject Each(Lambda action);

   IObject Rotate(int count);

   IObject Shift(int count);

   IObject Shift(int count, IObject defaultValue);

   IObject Permutations(int count);

   IObject Permutations();

   IObject Combinations(int count);

   IObject Combinations();

   IObject Flatten();

   IObject Copy();

   IObject Apply(ICollection collection);

   IObject Column(int column);

   IObject Partition(Lambda lambda);

   IObject Pick(int count);

   IObject Pick();

   IObject Roll(int count);

   IObject Splat(int count);

   IObject Chunked(int count);

   IObject Windowed(int size, int step, bool partial);

   IObject Repeated();

   IObject Accumulate(Lambda lambda);

   IObject Accumulate(IObject initialValue, Lambda lambda);

   KBoolean AllTrue(IObject argument);

   KBoolean AnyTrue(IObject argument);

   KBoolean NoneTrue(IObject argument);

   KTuple HeadTail();

   Junction JunctionAll();

   Junction JunctionAny();

   Junction JunctionNone();

   Junction JunctionOne();

   IObject Step(int step);

   IObject this[int index] { get; }

   Sequence Seq();

   IObject Transpose();

   IObject Assoc(IObject target);

   IObject At(int index);
}