using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class FloatParser : SymbolParser
   {
      public override string Pattern => "^ /(|s|) /([/d '_']+ '.' [/d '_']+) /'e' /(['-+']? /d+)? /'i'?";

      public FloatParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var source = tokens[2].Text.Replace("_", "") + tokens[3].Text + tokens[4].Text;
         var type = tokens[5].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Number, Color.NumberPart, Color.Number, Color.NumberPart);

         if (double.TryParse(source, out var result))
         {
            if (type == "i")
               builder.Add(new ComplexSymbol(result));
            else
               builder.Add(new FloatSymbol(result));
            return Unit.Matched();
         }
         else
            return failedMatch<Unit>(unableToConvert(source, "Float"));
      }
   }
}