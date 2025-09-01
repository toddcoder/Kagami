using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class CalculatedReturnParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(return)(\s*)(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.OpenParenthesis);

      var _result =
         from taggedExpressionValue in getTaggedExpressions(state, REGEX_EXP_END)
         from expressionValue in getExpression(state, ExpressionFlags.Standard)
         select (taggedExpressionValue, expressionValue);
      if (_result is (true, var (taggedExpressions, expression)))
      {
         state.AddStatement(new CalculatedReturn(taggedExpressions, expression));
         state.CommitTransaction();

         return unit;
      }
      else
      {
         state.RollBackTransaction();
         return _result.Exception;
      }
   }
}