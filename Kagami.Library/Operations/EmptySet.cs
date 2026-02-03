using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class EmptySet(Maybe<TypeConstraint> _typeConstraint) : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var set = Set.Empty;
      set.TypeConstraint = _typeConstraint;

      return set;
   }

   public override string ToString() => "empty.set";
}