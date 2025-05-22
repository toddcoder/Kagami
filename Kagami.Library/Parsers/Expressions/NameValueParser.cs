using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class NameValueParser : EndingInExpressionParser
{
   protected string name = "";

   public NameValueParser(ExpressionBuilder builder) : base(builder, ExpressionFlags.OmitColon | ExpressionFlags.OmitComma) { }

   public override string Pattern => $"^ /(/s*) /({REGEX_FIELD}) /':'";

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      name = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Label, Color.Operator);

      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      builder.Add(new NameValueSymbol(name, expression));
      return unit;
   }
}