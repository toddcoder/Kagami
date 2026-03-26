using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class AbstractPropertyParser : StatementParser
{
   [GeneratedRegex($@"^(\s*)(abstract)(\s+)(get|set)(\s+)({REGEX_FIELD})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var direction = tokens[4].Text;
      var propertyName = tokens[6].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier);

      var _possibleTypeConstraint = parseTypeConstraint(state);
      if (_possibleTypeConstraint is (true, var possibleTypeConstraint))
      {
         var _typeConstraint = possibleTypeConstraint.Maybe;
         var block = new Block(new AbstractFail(propertyName), _typeConstraint);
         (propertyName, var parameters) = PropertyParser.PropertyNameParameters(state, direction, propertyName, _typeConstraint);
         var function = new Function(propertyName, parameters, false, block, false, false, "");
         state.AddStatement(function);

         return unit;
      }
      else
      {
         return _possibleTypeConstraint.Exception;
      }
   }
}