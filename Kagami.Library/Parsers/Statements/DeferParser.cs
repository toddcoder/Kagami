using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using Core.RegularExpressions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class DeferParser : StatementParser
   {
      public override string Pattern => "^ /'defer' /b";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword);

         Block block;
         if (state.CurrentSource.IsMatch(REGEX_EOL))
         {
            if (getBlock(state).ValueOrCast<Unit>(out block, out var asUnit)) { }
            else
            {
               return asUnit;
            }
         }
         else if (getExpression(state, ExpressionFlags.Standard).ValueOrCast<Unit>(out var expression, out var asUnit))
         {
            block = new Block(new ExpressionStatement(expression, true));
         }
         else
         {
            return asUnit;
         }

         block.AddReturnIf();
         state.AddStatement(new Defer(block));

         return Unit.Matched();
      }
   }
}