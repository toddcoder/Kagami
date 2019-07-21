using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class MatchItemSymbol : Symbol
	{
		Expression comparisand;
		Expression result;

		public MatchItemSymbol(Expression comparisand, Expression result)
		{
			this.comparisand = comparisand;
			this.result = result;
		}

		public override void Generate(OperationsBuilder builder)
		{
			comparisand.Generate(builder);
			result.Generate(builder);
			builder.Match();
		}

		public override Precedence Precedence => Precedence.Value;

		public override Arity Arity => Arity.Nullary;
	}
}