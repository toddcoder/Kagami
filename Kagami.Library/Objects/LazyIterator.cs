using System.Linq;
using Core.Enumerables;

namespace Kagami.Library.Objects
{
   public class LazyIterator : Iterator
   {
      public LazyIterator(ICollection collection) : base(collection) { }

      public override string ClassName => "LazyIterator";

      public override string Image => $"^{collection.GetIterator(false).List().Select(i => i.Image).Listify()}";

      public override bool IsLazy => true;

      public override IObject Map(Lambda lambda) => new StreamIterator(this, new MapAction(lambda));

      public override IObject If(Lambda predicate) => new StreamIterator(this, new IfAction(predicate));

      public override IObject Skip(int count) => new StreamIterator(this, new SkipAction(count));

      public override IObject SkipWhile(Lambda predicate) => new StreamIterator(this, new SkipWhileAction(predicate));

      public override IObject SkipUntil(Lambda predicate) => new StreamIterator(this, new SkipUntilAction(predicate));

      public override IObject Take(int count) => new StreamIterator(this, new TakeAction(count));

      public override IObject TakeWhile(Lambda predicate) => new StreamIterator(this, new TakeWhileAction(predicate));

      public override IObject TakeUntil(Lambda predicate) => new StreamIterator(this, new TakeUntilAction(predicate));

      public override IObject Distinct() => new StreamIterator(this, new DistinctAction());
   }
}