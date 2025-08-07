using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Iterators;

public class StreamingIterator(IIterator iterator) : IObject, IIterator
{
   protected IObject collectionAsObject = (IObject)iterator.Collection;
   protected ICollectionClass collectionClass = iterator.CollectionClass;
   protected List<StreamingAction> actions = [];

   public string ClassName => "StreamingIterator";

   public string AsString => actions.ToString(" ");

   public string Image => AsString;

   public int Hash => actions.GetHashCode();

   public bool IsEqualTo(IObject obj) => collectionAsObject.IsEqualTo(obj);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => collectionAsObject.IsTrue;

   public Guid Id { get; init; } = Guid.NewGuid();

   public TypeConstraint TypeConstraint() => TODO_IMPLEMENT_ME;

   public ICollection Collection => iterator.Collection;

   public ICollectionClass CollectionClass => iterator.CollectionClass;

   public bool IsLazy => true;

   public Maybe<IObject> Next() => TODO_IMPLEMENT_ME;

   public Maybe<IObject> Peek() => TODO_IMPLEMENT_ME;

   public IObject Reset() => TODO_IMPLEMENT_ME;

   public IEnumerable<IObject> List() => TODO_IMPLEMENT_ME;

   public IIterator Clone() => TODO_IMPLEMENT_ME;

   public IObject Reverse() => TODO_IMPLEMENT_ME;

   public KString Join() => TODO_IMPLEMENT_ME;

   public KString Join(string connector) => TODO_IMPLEMENT_ME;

   public KString Join(string connector, int limit, string truncated) => TODO_IMPLEMENT_ME;

   public IObject Join(Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject Sort(Lambda lambda, bool ascending) => TODO_IMPLEMENT_ME;

   public IObject Sort(bool ascending) => TODO_IMPLEMENT_ME;

   public IObject FoldLeft(IObject initialValue, Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject FoldLeft(Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject FoldRight(IObject initialValue, Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject FoldRight(Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject ReduceLeft(IObject initialValue, Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject ReduceLeft(Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject ReduceRight(IObject initialValue, Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject ReduceRight(Lambda lambda) => TODO_IMPLEMENT_ME;

   public Int Count() => TODO_IMPLEMENT_ME;

   public Int Count(IObject item) => TODO_IMPLEMENT_ME;

   public Int Count(Lambda predicate) => TODO_IMPLEMENT_ME;

   public IObject Map(Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject FlatMap(Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject Replace(Lambda predicate, Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject If(Lambda predicate) => TODO_IMPLEMENT_ME;

   public IObject IfNot(Lambda predicate) => TODO_IMPLEMENT_ME;

   public IObject Skip(int count) => TODO_IMPLEMENT_ME;

   public IObject SkipWhile(Lambda predicate) => TODO_IMPLEMENT_ME;

   public IObject SkipUntil(Lambda predicate) => TODO_IMPLEMENT_ME;

   public IObject Take(int count) => TODO_IMPLEMENT_ME;

   public IObject TakeWhile(Lambda predicate) => TODO_IMPLEMENT_ME;

   public IObject TakeUntil(IObject obj) => TODO_IMPLEMENT_ME;

   public IObject Index(Lambda predicate) => TODO_IMPLEMENT_ME;

   public IObject Indexes(Lambda predicate) => TODO_IMPLEMENT_ME;

   public IObject Zip(ICollection collection) => TODO_IMPLEMENT_ME;

   public IObject Zip(ICollection collection, Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject ZipL(ICollection collection, IObject leftDefaultValue, IObject rightDefaultValue) => TODO_IMPLEMENT_ME;

   public IObject ZipL(ICollection collection, IObject leftDefaultValue, IObject rightDefaultValue, Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject Unzip() => TODO_IMPLEMENT_ME;

   public IObject Unzip(Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject Min() => TODO_IMPLEMENT_ME;

   public IObject Min(Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject Max() => TODO_IMPLEMENT_ME;

   public IObject Max(Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject First() => TODO_IMPLEMENT_ME;

   public IObject First(Lambda predicate) => TODO_IMPLEMENT_ME;

   public IObject Last() => TODO_IMPLEMENT_ME;

   public IObject Last(Lambda predicate) => TODO_IMPLEMENT_ME;

   public IObject Split(Lambda predicate) => TODO_IMPLEMENT_ME;

   public IObject Split(int count) => TODO_IMPLEMENT_ME;

   public IObject GroupBy(Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject GroupBy(Lambda keyLambda, Lambda valueLambda) => TODO_IMPLEMENT_ME;

   public KBoolean One(Lambda predicate) => TODO_IMPLEMENT_ME;

   public KBoolean None(Lambda predicate) => TODO_IMPLEMENT_ME;

   public KBoolean Any(Lambda predicate) => TODO_IMPLEMENT_ME;

   public KBoolean All(Lambda predicate) => TODO_IMPLEMENT_ME;

   public INumeric Sum() => TODO_IMPLEMENT_ME;

   public INumeric Average() => TODO_IMPLEMENT_ME;

   public INumeric Product() => TODO_IMPLEMENT_ME;

   public IObject Cross(ICollection collection) => TODO_IMPLEMENT_ME;

   public IObject Cross(ICollection collection, Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject By(int count) => TODO_IMPLEMENT_ME;

   public IObject Window(int count) => TODO_IMPLEMENT_ME;

   public IObject Shape(int rows, int columns) => TODO_IMPLEMENT_ME;

   public IObject Unique() => TODO_IMPLEMENT_ME;

   public IObject Unique(Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject Span(Lambda predicate) => TODO_IMPLEMENT_ME;

   public IObject Span(int count) => TODO_IMPLEMENT_ME;

   public IObject Shuffle() => TODO_IMPLEMENT_ME;

   public IObject Random() => TODO_IMPLEMENT_ME;

   public IObject Collect() => TODO_IMPLEMENT_ME;

   public KArray ToArray() => TODO_IMPLEMENT_ME;

   public List ToList() => TODO_IMPLEMENT_ME;

   public KTuple ToTuple() => TODO_IMPLEMENT_ME;

   public Dictionary ToDictionary(Lambda keyLambda, Lambda valueLambda) => TODO_IMPLEMENT_ME;

   public IObject ToDictionary() => TODO_IMPLEMENT_ME;

   public IObject ToSet() => TODO_IMPLEMENT_ME;

   public IObject Each(Lambda action) => TODO_IMPLEMENT_ME;

   public IObject Rotate(int count) => TODO_IMPLEMENT_ME;

   public IObject Permutations(int count) => TODO_IMPLEMENT_ME;

   public IObject Permutations() => TODO_IMPLEMENT_ME;

   public IObject Combinations(int count) => TODO_IMPLEMENT_ME;

   public IObject Combinations() => TODO_IMPLEMENT_ME;

   public IObject Flatten() => TODO_IMPLEMENT_ME;

   public IObject Copy() => TODO_IMPLEMENT_ME;

   public IObject Apply(ICollection collection) => TODO_IMPLEMENT_ME;

   public IObject Column(int column) => TODO_IMPLEMENT_ME;

   public IObject Partition(Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject Pick(int count) => TODO_IMPLEMENT_ME;

   public IObject Roll(int count) => TODO_IMPLEMENT_ME;

   public IObject Splat(int count) => TODO_IMPLEMENT_ME;

   public IObject Chunked(int count) => TODO_IMPLEMENT_ME;

   public IObject Windowed(int size, int step, bool partial) => TODO_IMPLEMENT_ME;

   public IObject Repeated() => TODO_IMPLEMENT_ME;

   public IObject Accumulate(Lambda lambda) => TODO_IMPLEMENT_ME;

   public IObject Accumulate(IObject initialValue, Lambda lambda) => TODO_IMPLEMENT_ME;

   public KBoolean AllTrue(IObject argument) => TODO_IMPLEMENT_ME;

   public KBoolean AnyTrue(IObject argument) => TODO_IMPLEMENT_ME;

   public KBoolean NoneTrue(IObject argument) => TODO_IMPLEMENT_ME;
}