using System.Text.RegularExpressions;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class BinaryParser : SymbolParser
{
   public BinaryParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(0b)([01][01_`]*)([Lif])?\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var source = tokens[3].Text;
      var type = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.NumberPart, Color.Number, Color.NumberPart);

      var number = convert(source.Replace("_", "").Replace("`", ""), 2, "01");
      return getNumber(builder, type, number);
   }
}