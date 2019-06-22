using System;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class HexExpParser : SymbolParser
   {
      public override string Pattern => "^ /(|s|) /'0x' /([/d '_a-f']+ '.' [/d '_a-f']+) (/'p' /(['-+']? /d+))? /'i'?";

      public HexExpParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var source = tokens[3].Text.Replace("_", "") + tokens[4].Text + tokens[5].Text;
         var type = tokens[6].Text;
         state.Colorize(tokens, Color.Whitespace, Color.NumberPart, Color.Number, Color.NumberPart, Color.Number, Color.NumberPart);

         var prefix = source.Contains("p") ? source.KeepUntil("p") : source;
         var suffix = source.Contains("p") ? source.DropUntil("p").Drop(1) : "";

         var left = convertFloat(prefix, 16, "0123456789abcdef");

         if (suffix.IsEmpty())
         {
            if (type == "i")
            {
	            builder.Add(new ComplexSymbol(left));
            }
            else
            {
	            builder.Add(new FloatSymbol(left));
            }

            return Unit.Matched();
         }
         else if (int.TryParse(suffix, out var right))
         {
            var raised = Math.Pow(2, right);
            var result = left * raised;
            if (type == "i")
            {
	            builder.Add(new ComplexSymbol(result));
            }
            else
            {
	            builder.Add(new FloatSymbol(result));
            }

            return Unit.Matched();
         }
         else
         {
	         return failedMatch<Unit>(unableToConvert(suffix, "Long"));
         }
      }
   }
}