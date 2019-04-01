using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

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
			state.BeginTransaction();

			mutable = tokens[1].Text == "var";
			fieldName = tokens[3].Text;
			state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier);

			if (parseTypeConstraint(state).If(out typeConstraint, out var mbException)) { }
			else if (mbException.If(out var exception))
				return failedMatch<Unit>(exception);
			else
				typeConstraint = none<TypeConstraint>();

			if (state.Scan("^ /(|s|) /'='", Color.Whitespace, Color.Structure).If(out _, out mbException))
			{
				state.CommitTransaction();
				return Unit.Matched();
			}
			else if (mbException.If(out var exception))
				return failedMatch<Unit>(exception);
			else
			{
				state.RollBackTransaction();
				return notMatched<Unit>();
         }
      }

		public override IMatched<Unit> Suffix(ParseState state, Expression expression)
		{
			state.AddStatement(new AssignToNewField(mutable, fieldName, expression, typeConstraint));
			return Unit.Matched();
		}
	}
}