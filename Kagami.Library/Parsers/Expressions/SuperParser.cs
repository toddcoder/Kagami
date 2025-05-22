using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class SuperParser : SymbolParser
{
   public SuperParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => "^ /(/s*) /'super' /b";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      builder.Add(new SuperSymbol());

      return unit;
   }
}