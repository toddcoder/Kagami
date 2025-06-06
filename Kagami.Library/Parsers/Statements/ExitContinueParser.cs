using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ExitContinueParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(exit|continue)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      Statement statement = tokens[1].Text == "exit" ? new Exit() : new Continue();
      state.AddStatement(statement);

      return unit;
   }
}