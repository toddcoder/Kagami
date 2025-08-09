using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Inclusions;
using Kagami.Library.Nodes.Statements;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class RequiredFieldParser(Inclusion inclusion) : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(required)(\s+)(var|let)(\s+)({REGEX_FIELD})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var mutable = tokens[4].Text == "var";
      var fieldName = tokens[6].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier);

      var _possibleTypeConstraint = parseTypeConstraint(state);
      if (_possibleTypeConstraint is (true, var possibleTypeConstraint))
      {
         var _typeConstraint = possibleTypeConstraint.Maybe;
         var requiredField = new RequiredField(fieldName, mutable, _typeConstraint);
         state.AddStatement(requiredField);

         return inclusion.Register(requiredField).Optional();
      }
      else
      {
         return _possibleTypeConstraint.Exception;
      }
   }
}