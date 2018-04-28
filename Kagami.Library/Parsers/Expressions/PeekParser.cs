using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class PeekParser : SymbolParser
   {
      public PeekParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'peek' /'('";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Invokable, Color.Structure);
         return getArguments(state, builder.Flags).Map(e =>
         {
            builder.Add(new PeekSymbol(e[0]));
            return Unit.Matched();
         });
      }
   }
}