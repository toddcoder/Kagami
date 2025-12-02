using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class RequireParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(require|reject)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var not = tokens[2].Text == "reject";
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var _expression = getExpression(state, ExpressionFlags.Standard);
      if (_expression is (true, var expression))
      {
         if (not)
         {
            var _block = getBlock(state);
            if (_block is (true, var block))
            {
               state.AddStatement(new If(expression, block));
               return unit;
            }
            else
            {
               return _block.Exception;
            }
         }
         else
         {
            var block = new Block(new Pass());
            var _elseBlock =
               from keyword in state.Scan(@"^(\s+)(else)", Color.Whitespace, Color.Keyword)
               from eBlock in getBlock(state)
               select eBlock;
            if (_elseBlock is (true, var elseBlock))
            {
               state.AddStatement(new If(expression, block, nil, elseBlock, "", false, false, true, true));
               return unit;
            }
            else
            {
               return _elseBlock.Exception;
            }
         }
      }
      else
      {
         return _expression.Exception;
      }
   }
}