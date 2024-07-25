using Kagami.Library.Nodes.Symbols;
using Core.Monads;

namespace Kagami.Library.Parsers.Expressions
{
   public class LazyParser : EndingInExpressionParser
   {
      public LazyParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'lazy' /b";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword);
         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         builder.Add(new LazySymbol(expression));
         return Unit.Matched();
      }
   }
}