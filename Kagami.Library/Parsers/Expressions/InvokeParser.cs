using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class InvokeParser : SymbolParser
   {
      public InvokeParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => $"^ /(|s|) /({REGEX_FUNCTION_NAME}) /'('";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var functionName = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Invokable, Color.Structure);

         if (getArgumentsPlusLambda(state, builder.Flags).If(out var tuple, out var original))
         {
            var (arguments, possibleLambda) = tuple;

            if (state.Macro(functionName).If(out var function))
               builder.Add(new MacroInvokeSymbol(function, arguments, possibleLambda));
            else
               builder.Add(new InvokeSymbol(functionName, arguments, possibleLambda));

            return Unit.Matched();
         }
         else
            return original.Unmatched<Unit>();
      }
   }
}