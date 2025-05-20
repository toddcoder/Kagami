using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class WhateverParser : SymbolParser
{
   public WhateverParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(|s|) /'#'";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Identifier);
      builder.Add(new WhateverSymbol());

      return unit;
   }
}