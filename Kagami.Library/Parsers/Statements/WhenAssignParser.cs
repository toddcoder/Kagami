using Core.Monads;
using Core.Strings;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class WhenAssignParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(when)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var not = state.Scan(@"^\b(not)\b", Color.Keyword).Map(n => n.IsNotEmpty()) | false;

      var _result =
         from comparisandValue in getExpression(state, ExpressionFlags.Comparisand | ExpressionFlags.OmitColon)
         from stem in state.Scan(@"^(\s+)(=)", Color.Whitespace, Color.Structure)
         from expressionValue in getExpression(state, ExpressionFlags.Standard)
         select (comparisandValue, expressionValue);
      if (_result is (true, var (comparisand, expression)))
      {
         var _elseBlock =
            from elseKeyword in state.Scan(@"^(\s*)(else)", Color.Whitespace, Color.Keyword).Maybe()
            from elseBlock in getBlock(state)
            select elseBlock;

         state.CommitTransaction();
         state.AddStatement(new MatchAssign(comparisand, expression, _elseBlock, not));

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