namespace Kagami.Library.Objects
{
   public class LazyIterator : Iterator
   {
      public LazyIterator(ICollection collection) : base(collection) { }

      public override string ClassName => "LazyIterator";

      public override string Image => "LazyIterator";//$"^{collection.GetIterator(false).List().Select(i => i.Image).Listify()}";

      public override bool IsLazy => true;

      public override IObject Map(Lambda lambda) => new StreamIterator(this).Map(lambda);

      public override IObject FlatMap(Lambda lambda) => new StreamIterator(this).FlatMap(lambda);

      public override IObject If(Lambda predicate) => new StreamIterator(this).If(predicate);

      public override IObject Skip(int count) => new StreamIterator(this).Skip(count);

      public override IObject SkipWhile(Lambda predicate) => new StreamIterator(this).SkipWhile(predicate);

      public override IObject SkipUntil(Lambda predicate) => new StreamIterator(this).SkipUntil(predicate);

      public override IObject Take(int count) => new StreamIterator(this).Take(count);

      public override IObject TakeWhile(Lambda predicate) => new StreamIterator(this).TakeWhile(predicate);

      public override IObject TakeUntil(Lambda predicate) => new StreamIterator(this).TakeUntil(predicate);

      public override IObject Distinct() => new StreamIterator(this).Distinct();
   }
}