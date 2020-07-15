using System.Collections.Generic;
using Kagami.Library.Classes;
using Core.Monads;

namespace Kagami.Library.Objects
{
   public interface IIterator : IEquivalentClass
   {
      ICollection Collection { get; }

      ICollectionClass CollectionClass { get; }

      bool IsLazy { get; }

      IMaybe<IObject> Next();

      IMaybe<IObject> Peek();

      IObject Reset();

      IEnumerable<IObject> List();

      IIterator Clone();

      IObject Reverse();

      String Join(string connector);

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

      Int Count(IObject item);

      Int Count(Lambda predicate);

      IObject Map(Lambda lambda);

      IObject FlatMap(Lambda lambda);

      IObject Replace(Lambda predicate, Lambda lambda);

      IObject If(Lambda predicate);

      IObject IfNot(Lambda predicate);

      IObject Skip(int count);

      IObject SkipWhile(Lambda predicate);

      IObject SkipUntil(Lambda predicate);

      IObject Take(int count);

      IObject TakeWhile(Lambda predicate);

      IObject TakeUntil(Lambda predicate);

      IObject Index(Lambda predicate);

      IObject Indexes(Lambda predicate);

      IObject Zip(ICollection collection);

      IObject Zip(ICollection collection, Lambda lambda);

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

      Boolean One(Lambda predicate);

      Boolean None(Lambda predicate);

      Boolean Any(Lambda predicate);

      Boolean All(Lambda predicate);

      INumeric Sum();

      INumeric Average();

      INumeric Product();

      IObject Cross(ICollection collection);

      IObject Cross(ICollection collection, Lambda lambda);

      IObject By(int count);

      IObject Window(int count);

      IObject Shape(int rows, int columns);

      IObject Distinct();

      IObject Span(Lambda predicate);

      IObject Span(int count);

      IObject Shuffle();

      IObject Random();

      IObject Collect();

      Array ToArray();

      List ToList();

      Tuple ToTuple();

      Dictionary ToDictionary(Lambda keyLambda, Lambda valueLambda);

      IObject ToDictionary();

      IObject ToSet();

      IObject Each(Lambda action);

      IObject Rotate(int count);

      IObject Permutation(int count);

      IObject Combination(int count);

      IObject Flatten();

      IObject Copy();

      IObject Apply(ICollection collection);

      IObject Column(int column);

      IObject Step(int step);
   }
}