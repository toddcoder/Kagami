using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class AssignToNewFieldParser : EndingInExpressionParser
	{
		bool mutable;
		string fieldName;
		IMaybe<TypeConstraint> typeConstraint;

		public override string Pattern => $"^ /('let' | 'var') /(/s+) /({REGEX_FIELD}) /b";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			mutable = tokens[1].Text == "var";
			fieldName = tokens[3].Text;
			state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier);

			if (parseTypeConstraint(state).If(out typeConstraint, out var isNotMatched, out var exception)) { }
			else if (isNotMatched)
				typeConstraint = none<TypeConstraint>();
			else
				return failedMatch<Unit>(exception);

			if (state.Scan("^ /(|s|) /'='", Color.Whitespace, Color.Structure).If(out _, out isNotMatched, out exception))
				return Unit.Matched();
			else if (isNotMatched)
				return notMatched<Unit>();
			else
				return failedMatch<Unit>(exception);
		}

		public override IMatched<Unit> Suffix(ParseState state, Expression expression)
		{
			state.AddStatement(new AssignToNewField(mutable, fieldName, expression, typeConstraint));
			return Unit.Matched();
		}
	}
}