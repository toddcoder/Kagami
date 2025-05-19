using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class NotParser : SymbolParser
{
   public NotParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(|s|) /'not' /b";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);
      builder.Add(new NotSymbol());

      return unit;
   }
}