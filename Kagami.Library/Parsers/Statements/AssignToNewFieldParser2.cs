﻿using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class AssignToNewFieldParser2 : EndingInExpressionParser
   {
      bool mutable;
      Expression comparisand;

      public override string Pattern => "^ /('let' | 'var') /(|s+|)";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         mutable = tokens[1].Text == "var";
         state.Colorize(tokens, Color.Keyword, Color.Whitespace);

         var result =
            from comparisand in getExpression(state, ExpressionFlags.Comparisand)
            from scanned in state.Scan("^ /(|s|) /'='", Color.Whitespace, Color.Structure)
            select comparisand;
         if (result.If(out comparisand, out var original))
            return Unit.Matched();
         else
            return original.Unmatched<Unit>();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         state.AddStatement(new AssignToNewField2(mutable, comparisand, expression));
         return Unit.Matched();
      }
   }
}