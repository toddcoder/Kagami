using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class ProtocolSetterParser(ProtocolBuilder builder) : StatementParser
{
   [GeneratedRegex($@"^(\s*)(set)(\s+)({REGEX_FIELD})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var propertyName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier);

      var _possibleTypeConstraint = parseTypeConstraint(state);
      if (_possibleTypeConstraint is (true, { Maybe: (true, var typeConstraint) }))
      {
         Selector typedSelector = $"{propertyName}=(_{typeConstraint.Image})";
         builder.AddSelector(typedSelector);
      }
      else
      {
         Selector setter = propertyName.set();
         builder.AddSelector(setter);
      }

      return unit;
   }
}