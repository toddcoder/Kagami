﻿using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class PostfixInvokeParser : SymbolParser
   {
      public override string Pattern => "^ /'('";

      public PostfixInvokeParser(ExpressionBuilder builder) : base(builder) {}

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.OpenParenthesis);

         return getArguments(state, builder.Flags).Map(arguments =>
         {
            builder.Add(new PostfixInvokeSymbol(arguments));
            return Unit.Value;
         });
      }
   }
}