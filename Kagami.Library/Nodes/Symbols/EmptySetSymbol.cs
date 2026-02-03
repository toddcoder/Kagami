using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class EmptySetSymbol(Maybe<TypeConstraint> _typeConstraint) : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.EmptySet(_typeConstraint);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => "{}";
}