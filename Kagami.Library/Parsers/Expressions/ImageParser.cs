using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Expressions
{
   public class ImageParser : SymbolParser
   {
      public ImageParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'~' -(> /s)";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator);
         builder.Add(new ImageSymbol());

         return Unit.Matched();
      }
   }
}