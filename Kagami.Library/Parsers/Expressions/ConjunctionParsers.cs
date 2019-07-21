using System.Collections.Generic;

namespace Kagami.Library.Parsers.Expressions
{
	public class ConjunctionParsers : MultiParser
	{
		ExpressionBuilder builder;

		public ConjunctionParsers(ExpressionBuilder builder) => this.builder = builder;

		public override IEnumerable<Parser> Parsers
		{
			get
			{
				yield return new EndOfCurrentExpressionParser(builder);
				//yield return new IfSomeNoneSymbolParser(builder);
				yield return new MatchExpressionParser(builder);

				if (!builder.Flags[ExpressionFlags.OmitComprehension])
				{
					yield return new ComprehensionParser(builder);
				}

				if (!builder.Flags[ExpressionFlags.OmitAnd])
				{
					yield return new AndParser(builder);
				}

				yield return new OrParser(builder);
				yield return new IsParser(builder);
				yield return new InlineIfParser(builder);
			}
		}
	}
}