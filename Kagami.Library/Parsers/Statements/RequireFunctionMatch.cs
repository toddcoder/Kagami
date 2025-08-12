using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Parsers.Statements;

public record RequireFunctionMatch(Selector Selector, Maybe<TypeConstraint> TypeConstraint)
{
   public bool Matches(Selector selector, Maybe<TypeConstraint> _typeConstraint)
   {
      return Selector.IsEqualTo(selector) && matchingTypeConstraints(TypeConstraint, _typeConstraint);
   }
}