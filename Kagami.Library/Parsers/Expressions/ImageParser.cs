using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ImageParser : SymbolParser
{
   public ImageParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(/s*) /'~' -(> /s)";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);
      builder.Add(new ImageSymbol());

      return unit;
   }
}