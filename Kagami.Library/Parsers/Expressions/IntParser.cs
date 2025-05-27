using System.Text.RegularExpressions;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class IntParser : SymbolParser
{
   public IntParser(ExpressionBuilder builder) : base(builder) { }

   //public override string Pattern => "^ /(/s*) /([/d '_']+) /['Lif']? /b";

   [GeneratedRegex(@"^(\s*)([\d_]+)([Lif])?\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var source = tokens[2].Text.Replace("_", "");
      var type = tokens[3].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Number, Color.NumberPart);

      return getNumber(builder, type, source);
   }
}