using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class ProtocolSetterParser(ProtocolBuilder builder) : StatementParser
{
   [GeneratedRegex($@"^(\s*)(set|var|get|let)(\s+)({REGEX_FIELD})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var type = tokens[2].Text;
      var hasGetter = type is "var" or "get";
      var hasSetter = type is "var" or "let" or "set";
      var propertyName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier);

      if (hasSetter)
      {
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
      }

      if (hasGetter)
      {
         var getter = propertyName.get();
         builder.AddSelector(getter);
      }

      return unit;
   }
}