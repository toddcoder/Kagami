using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class Join : TwoOperandOperation
{
   protected static KString join(IIterator iterator, string joinOn) => iterator.List().Select(i => i.AsString).ToString(joinOn);

   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y) => x switch
   {
      IIterator iterator => join(iterator, y.AsString),
      ICollection collection => join(collection.GetIterator(false), y.AsString),
      _ => expectedType("Iterator or Collection")
   };

   public override string ToString() => "join";
}