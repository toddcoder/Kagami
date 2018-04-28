using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Expressions
{
   public class BooleanParser : SymbolParser
   {
      public BooleanParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /('true' | 'false') /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Boolean);
         builder.Add(new BooleanSymbol(tokens[2].Text == "true"));

         return Unit.Matched();
      }
   }
}