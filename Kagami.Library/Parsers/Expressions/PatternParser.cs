using System.Collections.Generic;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class PatternParser : SymbolParser
   {
      public PatternParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'(' (> '|')";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Structure);

         var parser = new CaseExpressionParser(builder);
         var list = new List<(Expression, Expression)>();
         while (state.More)
            if (parser.Scan(state).If(out _, out var isNotMatched, out var exception))
               list.Add(parser.Expressions);
            else if (isNotMatched)
            {
               if (state.Scan("^ /')'", Color.Structure).If(out _, out isNotMatched, out exception))
               {
                  builder.Add(new PatternSymbol(list.ToArray()));
                  return Unit.Matched();
               }
               else if (isNotMatched)
                  return "Open pattern".FailedMatch<Unit>();
               else
                  return failedMatch<Unit>(exception);
            }
            else
               return failedMatch<Unit>(exception);

         return Unit.Matched();
      }
   }
}