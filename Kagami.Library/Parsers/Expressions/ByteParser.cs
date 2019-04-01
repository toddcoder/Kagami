using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class ByteParser : SymbolParser
   {
      public ByteParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /(/d1%3) /'b'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var source = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Number, Color.NumberPart);

         if (byte.TryParse(source, out var b))
         {
            builder.Add(new ByteSymbol(b));
            return Unit.Matched();
         }
         else
            return failedMatch<Unit>(unableToConvert(source, "Byte"));
      }
   }
}