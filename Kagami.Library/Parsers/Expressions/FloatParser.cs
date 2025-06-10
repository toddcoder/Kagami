using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class FloatParser : SymbolParser
{
   public FloatParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)([\d_]+\.[\d_]+)(?:(e)([-\+]?\d+))?(i|d)?")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var source = tokens[2].Text.Replace("_", "") + tokens[3].Text + tokens[4].Text;
      var type = tokens[5].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Number, Color.NumberPart, Color.Number, Color.NumberPart);

      if (type == "d")
      {
         if (decimal.TryParse(source, out var decimalResult))
         {
            builder.Add(new DecimalSymbol(decimalResult));
            return unit;
         }
         else
         {
            return unableToConvert(source, "Decimal");
         }
      }
      else if (double.TryParse(source, out var result))
      {
         if (type == "i")
         {
            builder.Add(new ComplexSymbol(result));
         }
         else
         {
            builder.Add(new FloatSymbol(result));
         }

         return unit;
      }
      else
      {
         return unableToConvert(source, "Float");
      }
   }
}