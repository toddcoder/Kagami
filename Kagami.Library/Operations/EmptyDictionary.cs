using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class EmptyDictionary(Maybe<TypeConstraint> _typeConstraint) : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var empty = Dictionary.Empty;
      empty.TypeConstraint = _typeConstraint;

      return empty;
   }

   public override string ToString() => "empty.dictionary";
}