using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class CommentParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(#.*)({REGEX_EOL})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Comment, Color.Whitespace);
      state.AddStatement(new Pass());

      return unit;
   }
}