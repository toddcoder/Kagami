using System;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class HexExpParser : SymbolParser
   {
      public override string Pattern => "^ /(|s|) /'0x' /([/d '_a-f']+) /'p' /(['-+']? /d+)? /'i'?";

      public HexExpParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var source = tokens[3].Text.Replace("_", "") + tokens[4].Text + tokens[5].Text;
         var type = tokens[6].Text;
         state.Colorize(tokens, Color.Whitespace, Color.NumberPart, Color.Number, Color.NumberPart, Color.Number, Color.NumberPart);

         var prefix = source.TakeUntil("p");
         var suffix = source.SkipUntil("p").Skip(1);

         var left = convert(prefix, 16, "0123456789abcdef");
         if (int.TryParse(suffix, out var right))
         {
            var raised = Math.Pow(2, right);
            var result = (double)left * raised;
            if (type == "i")
               builder.Add(new ComplexSymbol(result));
            else
               builder.Add(new FloatSymbol(result));

            return Unit.Matched();
         }
         else
            return failedMatch<Unit>(unableToConvert(suffix, "Long"));
      }
   }
}