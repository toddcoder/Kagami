using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class TryParser : EndingInExpressionParser
   {
      public TryParser(ExpressionBuilder builder, ExpressionFlags flags = ExpressionFlags.Standard) : base(builder, flags)
      {
      }

      public override string Pattern => "^ /(|s|) /'try' /b";

      public override Responding<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword);
         return unit;
      }

      public override Responding<Unit> Suffix(ParseState state, Expression expression)
      {
         builder.Add(new TrySymbol(expression));
         return unit;
      }
   }
}