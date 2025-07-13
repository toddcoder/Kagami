using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class AsOperatorParser : SymbolParser
{
   public AsOperatorParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex($@"^(\s+)(as)({REGEX_CLASS})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var className = tokens[3].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Class);
      builder.Add(new AsSymbol(className));

      return unit;
   }
}