using System.Text.RegularExpressions;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class PropertyParser : StatementParser
{
   public static (string propertyName, Parameters parameters) PropertyNameParameters(ParseState state, string direction, string propertyName,
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
         if (state.Scan(@"^\("))
         {
            var _actualParameters = getParameters(state);
            if (_actualParameters is (true, { Length: 1 } actualParameters))
            {
               return (propertyName, actualParameters);
            }
         }

         parameters = new Parameters(new Parameter(false, false, "", "value", nil, _typeConstraint, false, false, false));
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

      Maybe<Parameters> _parameters = nil;

      if (direction == "set" && state.Scan(@"^(\()", Color.OpenParenthesis))
      {
         var _setParameters = getParameters(state);
         if (_setParameters is (true, var setParameters))
         {
            if (setParameters.Length == 1)
            {
               _parameters = setParameters;
            }
            else
            {
               return fail("Set can only have one parameter");
            }
         }
         else
         {
            return _setParameters.Exception;
         }
      }

      state.CreateYieldFlag();
      state.CreateReturnType();
      var _block = getAnyBlock(state);
      if (_block is (true, var block))
      {
         var yielding = state.RemoveYieldFlag();
         state.RemoveReturnType();

         if (_parameters is (true, var parameters))
         {
            propertyName = $"{propertyName}=";
         }
         else
         {
            (propertyName, parameters) = PropertyNameParameters(state, direction, propertyName, block.TypeConstraint);
         }

         if (SelfAlias.IsNotEmpty())
         {
            block.InsertSelfAlias(SelfAlias);
         }

         state.AddStatement(new Function(propertyName, parameters, false, block, yielding, isOverride, ClassName));
         return unit;
      }
      else
      {
         return _block.Exception;
      }
   }

   public string ClassName { get; set; } = "";

   public string SelfAlias { get; set; } = "";
}