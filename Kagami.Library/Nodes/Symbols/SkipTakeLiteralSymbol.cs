using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class SkipTakeLiteralSymbol : Symbol
	{
		protected Expression literal;

		public SkipTakeLiteralSymbol(Expression literal) => this.literal = literal;

		public override void Generate(OperationsBuilder builder)
		{
			literal.Generate(builder);
			builder.SendMessage("literal()", 1);
		}

		public override Precedence Precedence => Precedence.PostfixOperator;

		public override Arity Arity => Arity.Postfix;
	}
}