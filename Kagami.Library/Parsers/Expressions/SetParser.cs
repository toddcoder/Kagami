﻿using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SetParser : SymbolParser
   {
      public SetParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(/s*) /'⎩' /(/s*)";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Collection, Color.Whitespace);

         if (getExpression(state, "^ /(/s*) /'⎭'", builder.Flags & ~ExpressionFlags.OmitComma, Color.Whitespace, Color.Collection)
            .ValueOrCast<Unit>(out var expression, out var asUnit))
         {
            builder.Add(new SetSymbol(expression));
            return Unit.Matched();
         }
         else
         {
            return asUnit;
         }
      }
   }
}