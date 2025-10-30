using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class EmptyArraySymbol(Maybe<TypeConstraint> _typeConstraint) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var array = KArray.Empty;
      array.TypeConstraint = _typeConstraint;
      builder.PushObject(array);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => "[]";
}