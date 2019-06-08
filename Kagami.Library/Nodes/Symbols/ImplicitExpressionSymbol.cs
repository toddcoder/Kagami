using Core.Monads;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class ImplicitExpressionSymbol : Symbol
	{
		Expression expression;
		string message;
		int parameterCount;
		Symbol symbol1;
		IMaybe<Symbol> symbol2;

		public ImplicitExpressionSymbol(Expression expression, string message, int parameterCount, Symbol symbol1, IMaybe<Symbol> symbol2)
		{
			this.expression = expression;
			this.message = message;
			this.parameterCount = parameterCount;
			this.symbol1 = symbol1;
			this.symbol2 = symbol2;
		}

		public override void Generate(OperationsBuilder builder)
		{
			var argumentCount = 0;
			symbol1.Generate(builder);
			if (symbol2.If(out var secondSymbol))
			{
				secondSymbol.Generate(builder);
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