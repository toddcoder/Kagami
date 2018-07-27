using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class MapExpressionParser : SymbolParser
   {
      public override string Pattern => "^ /(|s|) /['!&']";

      public MapExpressionParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var source = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Operator);

         if (getValue(state, builder.Flags).If(out var symbol, out var original))
         {
            var fieldName = newLabel("item");
            var tuple = (fieldName, symbol).Some();
            if (source == "!")
               state.MapExpression = tuple;
            else
               state.IfExpression = tuple;

            builder.Add(new FieldSymbol(fieldName));
            return Unit.Matched();
         }
         else
            return original.Unmatched<Unit>();
      }
   }
}