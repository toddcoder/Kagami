using System.Collections.Generic;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class PatternParser : SymbolParser
	{
		public PatternParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /'(' (> '|')";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Structure);

			var parser = new CaseExpressionParser(builder);
			var list = new List<(Expression, Expression)>();
			while (state.More)
				if (parser.Scan(state).If(out _, out var anyException))
					list.Add(parser.Expressions);
				else if (anyException.If(out var exception))
					return failedMatch<Unit>(exception);
				else if (state.Scan("^ /')'", Color.Structure).If(out _, out anyException))
				{
					builder.Add(new PatternSymbol(list.ToArray()));
					return Unit.Matched();
				}
				else if (anyException.If(out exception))
					return failedMatch<Unit>(exception);
				else
					return "Open pattern".FailedMatch<Unit>();

			return Unit.Matched();
		}
	}
}