using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Expressions
{
   public class SuperParser : SymbolParser
   {
      public SuperParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'super' /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword);
         builder.Add(new SuperSymbol());

         return Unit.Matched();
      }
   }
}