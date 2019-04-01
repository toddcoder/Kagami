using Core.Monads;
using Core.RegularExpressions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public abstract class StatementParser : Parser
	{
		public static IMaybe<int> ExceptionIndex { get; set; } = none<int>();

		public abstract IMatched<Unit> ParseStatement(ParseState state, Token[] tokens);

		public override IMatched<Unit> Scan(ParseState state)
		{
			state.BeginTransaction();
			if (IgnoreIndentation || SingleLine || state.Scan($"^ /({state.Indentation.FriendlyString()})", Color.Whitespace).IsMatched)
			{
				if (base.Scan(state).If(out _, out var mbException))
				{
					state.CommitTransaction();
					return Unit.Matched();
				}
				else if (mbException.If(out var exception))
				{
					ExceptionIndex = state.Index.Some();
					state.RollBackTransaction();
					return failedMatch<Unit>(exception);
				}
				else
				{
					state.RollBackTransaction();
					return notMatched<Unit>();
				}
			}
			else
			{
				state.RollBackTransaction();
				return notMatched<Unit>();
			}
		}

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens)
		{
			if (ParseStatement(state, tokens).Out(out _, out var original))
			{
				if (MatchEndOfLine)
					state.SkipEndOfLine();
				return Unit.Matched();
			}
			else
				return original;
		}

		public virtual bool MatchEndOfLine => true;

		public virtual bool IgnoreIndentation => false;

		public bool SingleLine { get; set; }

		protected StatementParser() : base(true) { }
	}
}