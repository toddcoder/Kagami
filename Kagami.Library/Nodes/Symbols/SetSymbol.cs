using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class SetSymbol : Symbol, IHasExpression
{
   protected Expression expression;

   public SetSymbol(Expression expression) => this.expression = expression;

   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);
      builder.NewSet();
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"[.{expression}.]";

   public Expression Expression => expression;
}