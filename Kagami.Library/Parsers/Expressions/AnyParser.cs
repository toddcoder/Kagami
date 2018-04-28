using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Expressions
{
   public class AnyParser : SymbolParser
   {
      public AnyParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'_' /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Identifier);
         builder.Add(new AnySymbol());

         return Unit.Matched();
      }
   }
}