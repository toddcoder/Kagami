﻿using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class WhileParser : StatementParser
   {
      public override string Pattern => "^ /'while' /b";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword);

         var result =
            from expression in getExpression(state, ExpressionFlags.Standard)
            from block in getBlock(state)
            select new While(expression, block);
         if (result.If(out var statement, out var original))
         {
            state.AddStatement(statement);
            return Unit.Matched();
         }
         else
            return original.Unmatched<Unit>();
      }
   }
}