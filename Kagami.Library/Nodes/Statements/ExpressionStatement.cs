using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Standard.Types.Booleans;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Nodes.Statements
{
	public class ExpressionStatement : Statement
	{
		Expression expression;
		bool returnExpression;
		IMaybe<TypeConstraint> typeConstraint;

		public ExpressionStatement(Expression expression, bool returnExpression, IMaybe<TypeConstraint> typeConstraint)
		{
			this.expression = expression;
			this.returnExpression = returnExpression;
			this.typeConstraint = typeConstraint;
		}

		public ExpressionStatement(Expression expression, bool returnExpression) :
			this(expression, returnExpression, none<TypeConstraint>()) { }

		public ExpressionStatement(Symbol symbol, bool returnExpression, IMaybe<TypeConstraint> typeConstraint)
		{
			expression = new Expression(symbol);
			this.returnExpression = returnExpression;
			this.typeConstraint = typeConstraint;
		}

		public ExpressionStatement(Symbol symbol, bool returnExpression) : this(symbol, returnExpression, none<TypeConstraint>()) { }

		public override void Generate(OperationsBuilder builder)
		{
			expression.Generate(builder);
			builder.Peek(Index);
			if (returnExpression)
			{
				if (typeConstraint.If(out var tc))
					builder.ReturnType(true, tc);
				else
					builder.Return(true);
			}
		}

		public Expression Expression => expression;

		public bool ReturnExpression => returnExpression;

		public override string ToString() => $"{returnExpression.Extend("return ")}{expression}";
	}
}