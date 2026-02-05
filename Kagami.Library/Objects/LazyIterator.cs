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

   public override IObject SkipWhile(Lambda predicate, bool back)
   {
      return back ? base.SkipWhile(predicate, back) : new StreamingIterator(this).SkipWhile(predicate, back);
   }

   public override IObject SkipUntil(Lambda predicate, bool back)
   {
      return back ? base.SkipUntil(predicate, back) : new StreamingIterator(this).SkipUntil(predicate, back);
   }

   public override IObject Take(int count) => new StreamingIterator(this).Take(count);

   public override IObject TakeWhile(Lambda predicate, bool back)
   {
      return back ? base.TakeWhile(predicate, back) : new StreamingIterator(this).TakeWhile(predicate, back);
   }
   public override IObject TakeUntil(Lambda predicate, bool back)
   {
      return back ? base.TakeUntil(predicate, back) : new StreamingIterator(this).TakeUntil(predicate, back);
   }

   public override IObject Zip(ICollection collection) => new StreamingIterator(this).Zip(collection);

   public override IObject Zip(ICollection collection, Lambda lambda) => new StreamingIterator(this).Zip(collection, lambda);

   public override IObject Unique() => new StreamingIterator(this).Unique();

   public override IObject Unique(Lambda lambda) => new StreamingIterator(this).Unique(lambda);

   public override IObject Each(Lambda action) => new StreamingIterator(this).Each(action);

   public override IObject By(int count) => new StreamingIterator(this).By(count);
}