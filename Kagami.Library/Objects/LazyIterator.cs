using Kagami.Library.Iterators;

namespace Kagami.Library.Objects;

public class LazyIterator : Iterator
{
   public LazyIterator(ICollection collection) : base(collection)
   {
   }

   public override string ClassName => "LazyIterator";

   public override string AsString => $"l{base.AsString}";

   public override string Image => $"l{base.Image}";

   public override bool IsLazy => true;

   public override IObject Map(Lambda lambda) => new StreamingIterator(this).Map(lambda);

   public override IObject FlatMap(Lambda lambda) => new StreamingIterator(this).FlatMap(lambda);

   public override IObject If(Lambda predicate) => new StreamingIterator(this).If(predicate);

   public override IObject Skip(int count) => new StreamingIterator(this).Skip(count);

   public override IObject SkipWhile(Lambda predicate) => new StreamingIterator(this).SkipWhile(predicate);

   public override IObject SkipUntil(Lambda predicate) => new StreamingIterator(this).SkipUntil(predicate);

   public override IObject Take(int count) => new StreamingIterator(this).Take(count);

   public override IObject TakeWhile(Lambda predicate) => new StreamingIterator(this).TakeWhile(predicate);

   public override IObject TakeUntil(Lambda predicate) => new StreamingIterator(this).TakeUntil(predicate);

   public override IObject Zip(ICollection collection) => new StreamingIterator(this).Zip(collection);

   public override IObject Zip(ICollection collection, Lambda lambda) => new StreamingIterator(this).Zip(collection, lambda);

   public override IObject Unique() => new StreamingIterator(this).Unique();

   public override IObject Unique(Lambda lambda) => new StreamingIterator(this).Unique(lambda);

   public override IObject Each(Lambda action) => new StreamingIterator(this).Each(action);

   public override IObject By(int count) => new StreamingIterator(this).By(count);
}