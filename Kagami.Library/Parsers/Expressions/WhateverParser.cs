using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;

namespace Kagami.Library.Parsers.Expressions
{
   public class WhateverParser : SymbolParser
   {
      public WhateverParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'#'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Identifier);
         builder.Add(new WhateverSymbol());

         return Unit.Matched();
      }
   }
}