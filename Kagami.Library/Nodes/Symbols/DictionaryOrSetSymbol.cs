using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class DictionaryOrSetSymbol(Expression expression, Maybe<TypeConstraint> _typeConstraint) : Symbol, IHasExpression
{
   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);
      builder.NewDictionaryOrSet(_typeConstraint);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public Expression Expression => expression;

   public override string ToString() => $"{{{expression}}}";
}