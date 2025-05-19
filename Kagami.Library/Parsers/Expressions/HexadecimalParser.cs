using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class HexadecimalParser : SymbolParser
{
   public HexadecimalParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(|s|) /'0x' /(['0-9a-f_']+) /['Li']? /b";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var source = tokens[3].Text;
      var type = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.NumberPart, Color.Number, Color.NumberPart);

      var number = convert(source.Replace("_", ""), 16, "0123456789abcdef");
      return getNumber(builder, type, number);
   }
}