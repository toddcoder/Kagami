using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Standard.Types.Booleans;

namespace Kagami.Library.Nodes.Statements
{
   public class ExpressionStatement : Statement
   {
      Expression expression;
      bool returnExpression;

      public ExpressionStatement(Expression expression, bool returnExpression)
      {
         this.expression = expression;
         this.returnExpression = returnExpression;
      }

      public ExpressionStatement(Symbol symbol, bool returnExpression)
      {
         expression = new Expression(symbol);
         this.returnExpression = returnExpression;
      }

      public override void Generate(OperationsBuilder builder)
      {
         expression.Generate(builder);
         builder.Peek(Index);
         if (returnExpression)
            builder.Return(true);
      }

      public Expression Expression => expression;

      public bool ReturnExpression => returnExpression;

      public override string ToString() => $"{returnExpression.Extend("return ")}{expression}";
   }
}