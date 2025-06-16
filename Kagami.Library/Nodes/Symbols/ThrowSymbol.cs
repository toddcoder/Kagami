using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ThrowSymbol : Symbol, IHasExpression
{
   protected Expression expression;

   public ThrowSymbol(Expression expression) => this.expression = expression;

   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);
      builder.Throw();
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"throw {expression}";

   public Expression Expression => expression;
}