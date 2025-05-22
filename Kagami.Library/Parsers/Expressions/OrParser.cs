using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class OrParser : EndingInExpressionParser
{
   public OrParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(/s*) /'or' /(/s+)";

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);
      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      builder.Add(new OrSymbol(expression));
      return unit;
   }
}