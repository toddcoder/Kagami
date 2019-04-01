using System.Collections.Generic;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class WhereParser : SymbolParser
	{
		public WhereParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'.{'";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Structure);

			var itemParser = new WhereItemParser(builder);
			var list = new List<(string, Expression)>();

			while (state.More)
			{
				var ((matched, _), (failed, exception)) = itemParser.Scan(state);
				if (matched)
				{
					list.Add((itemParser.PropertyName, itemParser.Expression));
					if (state.Scan("^ /(|s|) /[',}']", Color.Whitespace, Color.Structure).If(out var value))
					{
						if (value.Trim() == "}")
						{
							builder.Add(new WhereSymbol(list.ToArray()));
							return Unit.Matched();
						}
					}
					else
						return "Open where".FailedMatch<Unit>();
				}
				else if (failed)
					return failedMatch<Unit>(exception);
				else
					return notMatched<Unit>();
			}

			return Unit.Matched();
		}
	}
}