using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ForParser2 : StatementParser
   {
      public override string Pattern => "^ /'for' /(|s+|)";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword, Color.Whitespace);

         var result =
            from comparisand in getValue(state, ExpressionFlags.Comparisand)
            from scanned in state.Scan("^ /(|s|) /'<-'", Color.Whitespace, Color.Structure)
            from source in getExpression(state, ExpressionFlags.Standard)
            from block in getBlock(state)
            select (comparisand, source, block);
         if (result.If(out var tuple, out var original))
         {
            (var comparisand, var source, var block) = tuple;
            state.AddStatement(new For2(comparisand, source, block));
            return Unit.Matched();
         }

         return original.Unmatched<Unit>();
      }
   }
}