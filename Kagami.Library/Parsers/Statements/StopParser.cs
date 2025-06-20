using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class StopParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(stop)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      state.AddStatement(new Stop());

      return unit;
   }
}