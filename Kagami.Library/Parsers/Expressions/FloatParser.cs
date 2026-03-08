using System.Numerics;
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

   [GeneratedRegex(@"^(\s*)(\d[\d_`]*\.\d[\d_`]*)(?:([eE])([-\+]?\d+))?(i|d|f|r)?")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var source = tokens[2].Text.Replace("_", "").Replace("`", "") + tokens[3].Text + tokens[4].Text;
      var type = tokens[5].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Number, Color.NumberPart, Color.Number, Color.NumberPart);

      switch (type)
      {
         case "d" when decimal.TryParse(source, out var decimalResult):
         {
            builder.Add(new DecimalSymbol(decimalResult));
            return unit;
         }
         case "d":
            return unableToConvert(source, "Decimal");
         case "r":
         {
            var _decimalRational = getDecimalRational(source);
            if (_decimalRational is (true, var (numerator, denominator)))
            {
               builder.Add(new DecimalRationalSymbol(numerator, denominator));
               return unit;
            }
            else
            {
               return _decimalRational.Exception;
            }
         }
         default:
         {
            if (double.TryParse(source, out var result))
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

      static Optional<(BigInteger numerator, BigInteger denominator)> getDecimalRational(string source)
      {
         var s = source;
         var exp = 0;

         var eIndex = s.IndexOfAny(['e', 'E']);
         if (eIndex >= 0)
         {
            var expPart = s[(eIndex + 1)..];
            s = s[..eIndex];

            if (!int.TryParse(expPart, out exp))
            {
               return nil;
            }
         }

         var sign = 1;
         if (s.StartsWith('-'))
         {
            sign = -1;
            s = s[1..];
         }
         else if (s.StartsWith('+'))
         {
            s = s[1..];
         }

         var dotIndex = s.IndexOf('.');
         if (dotIndex < 0)
         {
            if (!BigInteger.TryParse(s, out var intVal))
            {
               return nil;
            }

            var numerator = intVal * sign;
            var denominator = BigInteger.One;

            switch (exp)
            {
               case > 0:
                  numerator *= BigInteger.Pow(10, exp);
                  break;
               case < 0:
                  denominator *= BigInteger.Pow(10, -exp);
                  break;
            }

            var gcd = gcdAbs(numerator, denominator);
            return (numerator / gcd, denominator / gcd);
         }

         var intPart = s[..dotIndex];
         var fracPart = s[(dotIndex + 1)..];

         var digits = intPart + fracPart;
         if (digits.Length == 0 || !BigInteger.TryParse(digits, out var rawNumerator))
         {
            return nil;
         }

         var numeratorResult = rawNumerator * sign;
         var denominatorResult = BigInteger.Pow(10, fracPart.Length);

         switch (exp)
         {
            case > 0:
               numeratorResult *= BigInteger.Pow(10, exp);
               break;
            case < 0:
               denominatorResult *= BigInteger.Pow(10, -exp);
               break;
         }

         var g = gcdAbs(numeratorResult, denominatorResult);
         return (numeratorResult / g, denominatorResult / g);
      }

      static BigInteger gcdAbs(BigInteger a, BigInteger b)
      {
         a = BigInteger.Abs(a);
         b = BigInteger.Abs(b);

         while (b != BigInteger.Zero)
         {
            var t = a % b;
            a = b;
            b = t;
         }

         return a == BigInteger.Zero ? BigInteger.One : a;
      }
   }
}