using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class AnnotationParser : StatementParser
{
   [GeneratedRegex(@"^@\w+")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens) => nil;
}