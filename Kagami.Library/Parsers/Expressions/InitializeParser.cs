using System.Collections.Generic;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class InitializeParser : SymbolParser
	{
		public InitializeParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => $"^ /(|s|) /({REGEX_CLASS}) /'{{'";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var className = tokens[2].Text;
			state.Colorize(tokens, Color.Whitespace, Color.Class, Color.Structure);

			var list = new List<(string, Expression)>();
			while (state.More)
			{
				var result =
					from f in state.Scan($"^ /(/s*) /({REGEX_FIELD}) /(|s|) /':'", Color.Whitespace, Color.Label, Color.Whitespace,
						Color.Structure)
					from e in getExpression(state, builder.Flags | ExpressionFlags.OmitComma | ExpressionFlags.OmitColon)
					from n in state.Scan("^ /(/s*) /[',}']", Color.Whitespace, Color.Structure)
					select (field: f.Trim().Skip(-1), expression: e, next: n);
				if (result.If(out var tuple, out var isNotMatched, out var exception))
				{
					list.Add((tuple.field, tuple.expression));
					if (tuple.next.Trim() == "}")
					{
						builder.Add(new InitializeSymbol(className, list.ToArray()));
						return Unit.Matched();
					}
				}
				else if (isNotMatched)
					return notMatched<Unit>();
				else
					return failedMatch<Unit>(exception);
			}

			return "Open initializer".FailedMatch<Unit>();
		}
	}
}