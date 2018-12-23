using Standard.Types.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class BinaryParser : SymbolParser
   {
      public BinaryParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'0b' /(['01_']+) /['Lif']? /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var source = tokens[3].Text;
         var type = tokens[4].Text;
         state.Colorize(tokens, Color.Whitespace, Color.NumberPart, Color.Number, Color.NumberPart);

         var number = convert(source.Replace("_", ""), 2, "01");
         return getNumber(builder, type, number);
      }
   }
}