using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class UntilParser : EndingInExpressionParser
{
   public override string Pattern => "^ /'until' /b";

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Keyword);
      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      Expression = expression;
      return unit;
   }

   public Expression Expression { get; set; }
}