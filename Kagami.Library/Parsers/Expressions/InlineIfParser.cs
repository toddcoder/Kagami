﻿using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class InlineIfParser : SymbolParser
   {
      public InlineIfParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s+|) /'?'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator);

         var result =
            from ifTrue in getExpression(state, builder.Flags | ExpressionFlags.OmitColon)
            from scanned in state.Scan("^ /(|s|) /':'", Color.Whitespace, Color.Operator)
            from ifFalse in getExpression(state, builder.Flags)
            select (ifTrue, ifFalse);

         if (result.ValueOrCast<Unit>(out var tuple, out var asUnit))
         {
            var (ifTrue, ifFalse) = tuple;
            builder.Add(new InlineIfSymbol(ifTrue, ifFalse));

            return Unit.Matched();
         }
         else
         {
            return asUnit;
         }
      }
   }
}