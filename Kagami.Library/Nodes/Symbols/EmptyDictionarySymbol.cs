using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class EmptyDictionarySymbol(Maybe<TypeConstraint> _typeConstraint) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var empty = Dictionary.Empty;
      empty.TypeConstraint = _typeConstraint;
      builder.PushObject(empty);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => "{:}";
}