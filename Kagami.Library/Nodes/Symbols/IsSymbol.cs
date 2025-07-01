using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class IsSymbol(Expression expression) : Symbol, IHasExpression
{
   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);
      builder.Match();
   }

   public override Precedence Precedence => Precedence.Boolean;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => $"is {expression}";

   public Expression Expression => expression;
}