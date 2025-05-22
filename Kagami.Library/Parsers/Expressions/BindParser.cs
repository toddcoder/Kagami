using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class BindParser : SymbolParser
{
   public BindParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(/s*) /'>>='";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);
      builder.Add(new BindSymbol());

      return unit;
   }
}