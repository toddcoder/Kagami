﻿using Core.Monads;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class OperatorsParser : SymbolParser
   {
      public OperatorsParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => $"^ /(|s|) /({REGEX_OPERATORS}1%2) -(>{REGEX_OPERATORS}) /(/s*)";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.BeginTransaction();

         var whitespace = tokens[1].Text.IsNotEmpty();
         var source = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Whitespace);

         if (getOperator(state, source, builder.Flags, whitespace).ValueOrCast<Unit>(out var symbol, out var asUnit))
         {
            builder.Add(symbol);
            state.CommitTransaction();

            return Unit.Matched();
         }
         else
         {
            state.RollBackTransaction();
            return asUnit;
         }
      }
   }
}