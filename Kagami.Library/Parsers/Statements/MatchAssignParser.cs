using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class MatchAssignParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(val)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      var _result =
         from comparisandValue in getExpression(state, ExpressionFlags.Comparisand | ExpressionFlags.OmitColon)
         from stem in state.Scan(@"^(\s+)(=)", Color.Whitespace, Color.Structure)
         from expressionValue in getExpression(state, ExpressionFlags.Standard)
         select (comparisandValue, expressionValue);
      if (_result is (true, var (comparisand, expression)))
      {
         state.CommitTransaction();
         state.AddStatement(new MatchAssign(comparisand, expression));

         return unit;
      }
      else if (_result.Exception is (true, var exception) && exception.Message != "Invalid expression syntax")
      {
         state.RollBackTransaction();
         return exception;
      }
      else
      {
         state.RollBackTransaction();
         return nil;
      }
   }
}