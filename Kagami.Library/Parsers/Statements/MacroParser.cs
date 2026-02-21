using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class MacroParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(macro)")]
   public override Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens) => unit;
}