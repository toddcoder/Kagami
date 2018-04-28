using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class AssignToMatchParser : EndingInExpressionParser
   {
      Symbol comparisand;

      public override string Pattern => "^ /'set' /(/s+)";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword, Color.Whitespace);

         var result =
            from comparisand in getValue(state, ExpressionFlags.Comparisand)
            from scanned in state.Scan("^ /(|s|) /'='", Color.Whitespace, Color.Structure)
            select comparisand;
         if (result.If(out comparisand, out var original))
            return Unit.Matched();
         else
            return original.Unmatched<Unit>();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         state.AddStatement(new AssignToMatch(comparisand, expression));
         return Unit.Matched();
      }
   }
}