using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class ReturnParser : EndingInExpressionParser
{
   public override string Pattern => "^ /(/s*) /'return' /(/s+)";

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);
      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      state.AddStatement(new Return(expression, state.GetReturnType()));
      return unit;
   }
}