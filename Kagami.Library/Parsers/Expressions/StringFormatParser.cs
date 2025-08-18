using System.Text.RegularExpressions;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class WhitespaceParser : SymbolParser
{
   public WhitespaceParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\$)([wt])(\d+)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var type = tokens[3].Text;
      var count = tokens[4].Text.Value().Int32();
      state.Colorize(tokens, Color.Whitespace, Color.Format, Color.Format, Color.Format);

      type = type == "w" ? " " : "\t";
      var str = type.Repeat(count);
      builder.Add(new StringSymbol(str));

      return unit;
   }
}