using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ConditionalAssignParser : StatementParser
   {
      public override string Pattern => "^ /('var' | 'let') /(|s|) /'if' /(|s+|)";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var mutable = tokens[1].Text == "var";
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Keyword, Color.Whitespace);

         var result =
            from comparisand in getExpression(state, ExpressionFlags.Comparisand)
            from scanned in state.Scan("^ /(|s|) /'='", Color.Whitespace, Color.Structure)
            from expression in getExpression(state, ExpressionFlags.Standard)
            from block in getBlock(state)
            select (comparisand, expression, block);

         if (result.If(out var tuple, out var original))
         {
            (var comparisand, var expression, var block) = tuple;
            var elseBlock = none<Block>();
            var elseParser = new ElseParser();
            if (elseParser.Scan(state).If(out _, out var isNotMatched, out var exception))
               elseBlock = elseParser.Block;
            else if (!isNotMatched)
               return failedMatch<Unit>(exception);

            state.AddStatement(new ConditionalAssign(mutable, comparisand, expression, block, elseBlock));
            return Unit.Matched();
         }
         else
            return original.Unmatched<Unit>();
      }
   }
}