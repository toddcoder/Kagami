﻿using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SymbolObjectParser : SymbolParser
   {
      public SymbolObjectParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => $"^ /(|s|) /'`' /({REGEX_FIELD}) /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var name = tokens[3].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Symbol, Color.Symbol);

         builder.Add(new SymbolSymbol(name));
         return Unit.Matched();
      }
   }
}