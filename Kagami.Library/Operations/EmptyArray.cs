using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class EmptyArray(Maybe<TypeConstraint> _typeConstraint) : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var array = KArray.Empty;
      array.TypeConstraint = _typeConstraint;

      return array;
   }

   public override string ToString() => "empty.array";
}