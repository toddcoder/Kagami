using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class UntilParser : EndingInExpressionParser
{
   //public override string Pattern => "^ /'until' /b";

   [GeneratedRegex(@"^(\s*)(until)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      Expression = expression;
      return unit;
   }

   public Expression Expression { get; set; } = Expression.Empty;
}