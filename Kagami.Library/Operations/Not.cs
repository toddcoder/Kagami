using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class Not : OneOperandOperation
{
   public override string ToString() => "not";

   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      KBoolean b => KBoolean.BooleanObject(!b.Value).Just(),
      Int i => (Int)(~i.Value),
      _ => incompatibleClasses(value, "Boolean or Int")
   };
}