using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class PropertyParser : StatementParser
{
   public static (string propertyName, Parameters parameters) PropertyNameParameters(string direction, string propertyName,
      Maybe<TypeConstraint> _typeConstraint)
   {
      Parameters parameters;
      if (direction == "get")
      {
         propertyName = $"__${propertyName}";
         parameters = Parameters.Empty;
      }
      else
      {
         propertyName = $"{propertyName}=";
         parameters = new Parameters(new Parameter(false, false, "", "value", nil, _typeConstraint, false, false));
      }

      return (propertyName, parameters);
   }

   [GeneratedRegex($@"^(\s*)(override\s+)?(get|set)(\s+)({REGEX_FIELD})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var isOverride = tokens[2].Text.StartsWith("override");
      var direction = tokens[3].Text;
      var propertyName = tokens[5].Text;

      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Identifier);

      state.CreateYieldFlag();
      state.CreateReturnType();
      var _block = getAnyBlock(state);
      if (_block is (true, var block))
      {
         var yielding = state.RemoveYieldFlag();
         state.RemoveReturnType();

         (propertyName, var parameters) = PropertyNameParameters(direction, propertyName, block.TypeConstraint);

         state.AddStatement(new Function(propertyName, parameters, false, block, yielding, isOverride, ""));
         return unit;
      }
      else
      {
         return _block.Exception;
      }
   }
}