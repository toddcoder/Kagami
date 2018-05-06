using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
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
            if (getBlock(state).If(out block, out var original)) { }
            else
               return original.UnmatchedOnly<Unit>();
         }
         else if (getExpression(state, ExpressionFlags.Standard).If(out var expression, out var exOriginal))
            block = new Block(new ExpressionStatement(expression, true));
         else
            return exOriginal.UnmatchedOnly<Unit>();

         block.AddReturnIf();
         state.AddStatement(new Defer(block));

         return Unit.Matched();
      }
   }
}