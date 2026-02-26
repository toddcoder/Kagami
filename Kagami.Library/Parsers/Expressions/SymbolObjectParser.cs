using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class SymbolObjectParser : SymbolParser
{
   public SymbolObjectParser(ExpressionBuilder builder) : base(builder)
   {
   }


   [GeneratedRegex($@"^(\s*)(@)(?!=\d)({REGEX_FIELD})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var name = tokens[3].Text;
      state.Colorize(tokens, Color.Whitespace, Color.SymbolPart, Color.Symbol);

      builder.Add(new SymbolSymbol(name));
      return unit;
   }
}