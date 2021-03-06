﻿using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class TryBlockParser : SymbolParser
   {
      public TryBlockParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => $"^ /(|s|) /'try' /({REGEX_EOL})";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);

         if (getBlock(state).ValueOrCast<Unit>(out var block, out var asUnit))
         {
            block.AddReturnIf(new UnitSymbol());
            var lambda = new LambdaSymbol(0, block);
            var invokeBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
            invokeBuilder.Add(lambda);
            invokeBuilder.Add(new PostfixInvokeSymbol(new Expression[0]));
            if (invokeBuilder.ToExpression().If(out var invokeExpression, out var exception))
            {
               builder.Add(new TrySymbol(invokeExpression));
               return Unit.Matched();
            }
            else
            {
               return failedMatch<Unit>(exception);
            }
         }
         else
         {
            return asUnit;
         }
      }
   }
}