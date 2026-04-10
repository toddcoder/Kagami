using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class ProtocolFieldParser(ProtocolBuilder builder) : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(var|let)(\s+)({REGEX_FIELD})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var createSetter = tokens[2].Text == "var";
      var fieldName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier);

      Selector getter = fieldName.get();
      builder.AddSelector(getter);

      if (createSetter)
      {
         var _possibleTypeConstraint = parseTypeConstraint(state);
         if (_possibleTypeConstraint is (true, { Maybe: (true, var typeConstraint) }))
         {
            Selector typedSetter = $"{fieldName}=(_{typeConstraint.Image})";
            builder.AddSelector(typedSetter);
         }
         else
         {
            Selector setter = fieldName.set();
            builder.AddSelector(setter);
         }
      }

      return unit;
   }
}