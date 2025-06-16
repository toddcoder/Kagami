using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class RepeatParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(repeat)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var _result =
         from expressionValue in getExpression(state, ExpressionFlags.Standard)
         from scanned in state.Scan(@"^(\s+)(times)\b", Color.Whitespace, Color.Keyword)
         from blockValue in getBlock(state)
         select (expressionValue, blockValue);

      if (_result is (true, var (expression, block)))
      {
         state.AddStatement(new Repeat(expression, block));
         return unit;
      }
      else
      {
         return nil;
      }
   }
}