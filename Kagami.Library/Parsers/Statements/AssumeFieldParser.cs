using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class AssumeFieldParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(assume\s+)(let|var)(\s+)({REGEX_FIELD})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var mutability = tokens[3].Text;
      var isMutable = mutability == "var";
      var fieldName = tokens[5].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Identifier);

      var _possibleTypeConstraint = parseTypeConstraint(state);
      if (_possibleTypeConstraint is (true, { Maybe: (true, var typeConstraint) }))
      {
         var defineNewField = new DefineNewField(isMutable, fieldName, typeConstraint, false, true, false);
         state.AddStatement(defineNewField);

         var function = Function.Getter(fieldName, typeConstraint, true);
         state.AddStatement(function);

         if (isMutable)
         {
            var setter = Function.Setter(fieldName, typeConstraint, true);
            state.AddStatement(setter);
         }

         return unit;
      }
      else

      {
         return fail("Type constraint required");
      }
   }
}