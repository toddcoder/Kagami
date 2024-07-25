using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class OctalParser : SymbolParser
   {
      public OctalParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'0o' /(['0-7_']+) /['Lif']? /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var source = tokens[3].Text;
         var type = tokens[4].Text;
         state.Colorize(tokens, Color.Whitespace, Color.NumberPart, Color.Number, Color.NumberPart);

         var number = convert(source.Replace("_", ""), 8, "01234567");
         return getNumber(builder, type, number);
      }
   }
}