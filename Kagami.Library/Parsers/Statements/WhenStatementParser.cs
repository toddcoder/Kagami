using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class WhenStatementParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(?:(var|let)(\s+)({REGEX_FIELD})(\s*)(=)(\s*))?(when)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var mutable = tokens[2].Text == "var";
      var fieldName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure, Color.Whitespace,
         Color.Keyword);

      var _beginBlock = state.BeginBlock();
      if (_beginBlock)
      {
         state.CreateReturnType();
         List<(Expression, Block)> expressionBlocks = [];
         while (state.More)
         {
            var _endBlock = state.EndBlock();
            if (_endBlock)
            {
               state.RemoveReturnType();
               break;
            }
            else if (_endBlock.Exception is (true, var exception))
            {
               return exception;
            }

            var _result =
               from expressionValue in getExpression(state, ExpressionFlags.Standard)
               from blockValue in getCaseStatementBlock(state)
               select (expressionValue, blockValue);
            if (_result is (true, var (expression, block)))
            {
               expressionBlocks.Add((expression, block));
            }
            else
            {
               return _result.Exception;
            }
         }

         state.AddStatement(new When([.. expressionBlocks], fieldName, mutable));
      }

      return unit;
   }
}