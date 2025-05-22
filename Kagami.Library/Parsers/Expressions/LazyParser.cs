using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class LazyParser : EndingInExpressionParser
{
   public LazyParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(/s*) /'lazy' /b";

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      builder.Add(new LazySymbol(expression));
      return unit;
   }
}