using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public abstract class StatementParser : PatternlessParser
{
   public static Optional<int> ExceptionIndex { get; set; } = nil;

   public abstract Optional<Unit> ParseStatement(ParseState state, Token[] tokens);

   public override Optional<Unit> Scan(ParseState state)
   {
      state.BeginTransaction();
      var _scan = base.Scan(state);
      if (_scan)
      {
         state.CommitTransaction();
         return unit;
      }
      else if (_scan.Exception is (true, var exception))
      {
         ExceptionIndex = state.Index;
         state.RollBackTransaction();

         return exception;
      }
      else
      {
         state.RollBackTransaction();
         return nil;
      }
   }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens)
   {
      var _statement = ParseStatement(state, tokens);
      if (_statement)
      {
         if (MatchEndOfLine)
         {
            state.SkipEndOfLine();
         }

         return unit;
      }
      else
      {
         return _statement.Exception;
      }
   }

   public virtual bool MatchEndOfLine => true;

   protected StatementParser() : base(true)
   {
   }
}