using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class SkipTakeInitLiteralSymbol : Symbol
	{
		Expression literal;

		public SkipTakeInitLiteralSymbol(Expression literal)
		{
			this.literal = literal;
		}

		public override void Generate(OperationsBuilder builder)
		{
			builder.NewSkipTake();
			literal.Generate(builder);
			builder.SendMessage("literal()", 1);
      }

		public override Precedence Precedence => Precedence.PostfixOperator;

		public override Arity Arity => Arity.Postfix;
	}
}