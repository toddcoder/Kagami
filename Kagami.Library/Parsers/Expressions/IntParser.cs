using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class IntParser : SymbolParser
{
   public IntParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(|s|) /([/d '_']+) /['Lif']? /b";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var source = tokens[2].Text.Replace("_", "");
      var type = tokens[3].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Number, Color.NumberPart);

      return getNumber(builder, type, source);
   }
}