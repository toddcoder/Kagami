using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class InParser : SymbolParser
{
   public InParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(|s|) (/'not' /(|s|))? /'in' /b";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var not = tokens[2].Text.Contains("not");
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Keyword);

      var message = not ? "notIn" : "in";
      builder.Add(new SendBinaryMessageSymbol(message, Precedence.Boolean, true));

      return unit;
   }
}