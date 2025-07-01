using Core.Monads;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ImplicitExpressionSymbol : Symbol, IHasExpression
{
   protected Expression expression;
   protected string message;
   protected int parameterCount;
   protected Symbol symbol1;
   protected Maybe<Symbol> _symbol2;

   public ImplicitExpressionSymbol(Expression expression, string message, int parameterCount, Symbol symbol1, Maybe<Symbol> _symbol2)
   {
      this.expression = expression;
      this.message = message;
      this.parameterCount = parameterCount;
      this.symbol1 = symbol1;
      this._symbol2 = _symbol2;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var argumentCount = 0;
      symbol1.Generate(builder);
      if (_symbol2 is (true, var symbol2))
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
   
   public Expression Expression => expression;
}