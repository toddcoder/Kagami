﻿using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Statements
{
   public class YieldParser : EndingInExpressionParser
   {
      public override string Pattern => "^ /'yield' /(|s+|)";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword, Color.Whitespace);
         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         state.AddStatement(new Yield(expression));
         state.SetYieldFlag();
         return Unit.Matched();
      }
   }
}