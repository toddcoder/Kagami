using Core.Monads;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class ImplicitExpressionSymbol : Symbol
   {
      protected Expression expression;
      protected string message;
      protected int parameterCount;
      protected Symbol symbol1;
      protected IMaybe<Symbol> _symbol2;

      public ImplicitExpressionSymbol(Expression expression, string message, int parameterCount, Symbol symbol1, IMaybe<Symbol> symbol2)
      {
         this.expression = expression;
         this.message = message;
         this.parameterCount = parameterCount;
         this.symbol1 = symbol1;
         _symbol2 = symbol2;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var argumentCount = 0;
         symbol1.Generate(builder);
         if (_symbol2.If(out var symbol2))
         {
            symbol2.Generate(builder);
            argumentCount++;
         }

         var lambda = new LambdaSymbol(parameterCount, expression);
         lambda.Generate(builder);
         argumentCount++;
         builder.SendMessage(message, argumentCount);
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;
   }
}