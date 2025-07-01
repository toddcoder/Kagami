using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class NameValueParser : EndingInExpressionParser
{
   protected string name = "";

   public NameValueParser(ExpressionBuilder builder) : base(builder, ExpressionFlags.OmitColon | ExpressionFlags.OmitComma)
   {
   }

   [GeneratedRegex($@"^(\s*)({REGEX_FIELD})(:)(\s+)")]
   public override partial Regex Regex();

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      name = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Label, Color.Operator, Color.Whitespace);

      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      builder.Add(new NameValueSymbol(name, expression));
      return unit;
   }
}