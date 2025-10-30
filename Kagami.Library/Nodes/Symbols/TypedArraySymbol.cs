using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class TypedArraySymbol(Expression expression, TypeConstraint typeConstraint) : Symbol, IHasExpression
{
   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);
      builder.NewTypedArray(typeConstraint);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public Expression Expression => expression;
}