using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class IsSymbol(Expression expression, bool not) : Symbol, IHasExpression
{
   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);
      builder.Match();
      if (not)
      {
         builder.PushBoolean(false);
         builder.Equal();
      }
   }

   public override Precedence Precedence => Precedence.Boolean;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => $"is {expression}";

   public Expression Expression => expression;
}