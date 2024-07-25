using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class IndexOptionalParser : SymbolParser
   {
      public IndexOptionalParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /'[?'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Structure);

         return getArguments(state, builder.Flags).Map(e =>
         {
            builder.Add(new SendMessageSymbol("[?]", e));
            return Unit.Value;
         });
      }
   }
}