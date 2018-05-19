using System;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using static Standard.Types.Maybe.MaybeFunctions;

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
            var (type, _, exception) = base.Scan(state).Values;
            switch (type)
            {
               case MatchType.Matched:
                  state.CommitTransaction();
                  return Unit.Matched();
               case MatchType.NotMatched:
                  state.RollBackTransaction();
                  return notMatched<Unit>();
               case MatchType.FailedMatch:
                  ExceptionIndex = state.Index.Some();
                  state.RollBackTransaction();
                  return failedMatch<Unit>(exception);
               default:
                  throw new ArgumentOutOfRangeException();
            }
         }

         state.RollBackTransaction();
         return notMatched<Unit>();
      }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens)
      {
         if (ParseStatement(state, tokens).If(out var _, out var original))
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