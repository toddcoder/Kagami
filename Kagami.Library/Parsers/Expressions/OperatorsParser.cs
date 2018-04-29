using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class OperatorsParser : SymbolParser
   {
      public OperatorsParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /(['-+*//\\%<=>!.~|?#@&^,.:']1%2) -(>['-+*//\\%<=>!.~|?#@&^,.:'])";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.BeginTransaction();

         var whitespace = tokens[1].Text.IsNotEmpty();
         var source = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Operator);

         switch (source)
         {
            case "+":
               builder.Add(new AddSymbol());
               break;
            case "-":
               builder.Add(new SubtractSymbol());
               break;
            case "*":
               builder.Add(new MultiplySymbol());
               break;
            case "/":
               if (whitespace)
                  builder.Add(new FloatDivideSymbol());
               else
                  builder.Add(new RationalSymbol());
               break;
            case "//":
               builder.Add(new IntDivideSymbol());
               break;
            case "/:":
               builder.Add(new SendBinaryMessageSymbol("foldl", Precedence.ChainedOperator));
               break;
            case @":\":
               builder.Add(new SendBinaryMessageSymbol("foldr", Precedence.ChainedOperator));
               break;
            case "%":
               builder.Add(new RemainderSymbol());
               break;
            case "%%":
               builder.Add(new RemainderZero());
               break;
            case "^":
               builder.Add(new RaiseSymbol());
               break;
            case "==":
               builder.Add(new EqualSymbol());
               break;
            case "!=":
               builder.Add(new NotEqualSymbol());
               break;
            case ">":
               builder.Add(new GreaterThanSymbol());
               break;
            case ">=":
               builder.Add(new GreaterThanEqualSymbol());
               break;
            case "<":
               builder.Add(new LessThanSymbol());
               break;
            case "<=":
               builder.Add(new LessThanEqualSymbol());
               break;
            case "++":
               builder.Add(new RangeSymbol(true));
               break;
            case "+-":
               builder.Add(new RangeSymbol(false));
               break;
            case "--":
               builder.Add(new RangeSymbol(true, true));
               break;
            case "::":
               builder.Add(new ConsSymbol());
               break;
            case "\\":
               builder.Add(new FormatSymbol());
               break;
            case ",":
               if (builder.Flags[ExpressionFlags.OmitComma])
               {
                  state.RollBackTransaction();
                  return notMatched<Unit>();
               }

               state.Scan("^ /(/s*)", Color.Whitespace);
               builder.Add(new CommaSymbol());
               break;
            case "~":
               builder.Add(new ConcatenationSymbol());
               break;
            case "<<":
            case ">>":
               builder.Add(new SendBinaryMessageSymbol(source, Precedence.Shift));
               break;
            case "=>":
               builder.Add(new KeyValueSymbol());
               break;
            case "|>":
               builder.Add(new PipelineSymbol());
               break;
            case "**":
               builder.Add(new OpenRangeSymbol());
               break;
            case "<>":
               builder.Add(new CompareSymbol());
               break;
            default:
               state.RollBackTransaction();
               return notMatched<Unit>();
         }

         state.CommitTransaction();
         return Unit.Matched();
      }
   }
}