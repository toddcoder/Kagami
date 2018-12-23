using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;

namespace Kagami.Library.Parsers.Expressions
{
   public class IfAsAndParser : EndingInExpressionParser
   {
      public IfAsAndParser(ExpressionBuilder builder) : base(builder, ExpressionFlags.OmitSendMessageAssign) { }

      public override string Pattern => "^ /(|s|) /'if' /(/s+)";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);
         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         builder.Add(new AndSymbol(expression));
         return Unit.Matched();
      }
   }
}