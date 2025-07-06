using Core.Computers;
using Core.Monads;
using Kagami.Library.Parsers.Statements;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class WhenParser(string matchFieldName, bool first) : StatementParser
{
   [GeneratedRegex(@"^(\s*)(when)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var not = state.NotKeyword();

      var _result =
         from comparisandValue in getCompoundComparisands(state, matchFieldName, not)
         from andValue in andExpression(state)
         from blockValue in getCaseStatementBlock(state)
         select (comparisandValue, andValue, blockValue);
      if (_result is (true, var (comparisand, possibleAnd, block)))
      {
         var builder = new ExpressionBuilder(ExpressionFlags.Standard);
         builder.Add(comparisand);
         if (possibleAnd.Maybe is (true, var and))
         {
            builder.Add(and);
         }

         var _expression = builder.ToExpression();
         if (_expression is (true, var expression))
         {
            var ifStatement = new If(expression, block);
         }
      }

      return unit;
   }
}