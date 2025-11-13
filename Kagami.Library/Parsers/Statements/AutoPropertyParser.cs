using Core.Monads;
using Core.Strings;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using System.Text.RegularExpressions;
using Kagami.Library.Invokables;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class AutoPropertyParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(?:(let|var)\s+)?(auto)(\s+)({REGEX_FIELD})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var type = tokens[2].Text;
      var name = tokens[5].Text;
      var isReadWrite = type.IsEmpty();
      var fieldName = $"_${name}";

      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Identifier);

      var _result =
         from possibleTypeConstraintValue in parseTypeConstraint(state)
         from scanned in state.Scan(@"^(\s*)(=)", Color.Whitespace, Color.Structure)
         from expressionValue in getExpression(state, ExpressionFlags.Standard)
         select (possibleTypeConstraintValue.Maybe, expressionValue);

      if (_result is (true, var (_typeConstraint, expression)))
      {
         var assignToNewField = new AssignToNewField(isReadWrite || type == "var", fieldName, expression, _typeConstraint, false) { Ignore = true };
         state.AddStatement(assignToNewField);

         if (isReadWrite || type is "let" or "var")
         {
            state.CreateYieldFlag();
            state.CreateReturnType();
            var _getter =
               from keyword in state.Scan(@"^(\s*)(get)\b", Color.Whitespace, Color.Keyword)
               from blockValue in getAnyBlock(state)
               select blockValue;
            if (_getter is (true, var getter))
            {
               state.RemoveYieldFlag();
               state.RemoveReturnType();
               var assign = new AssignReferenceToNewField(fieldName, "field");
               getter.Unshift(assign);
               state.AddStatement(new Function($"__${name}", Parameters.Empty, false, getter, false, false, ""));
            }
            else
            {
               state.AddStatement(Function.Getter($"__${name}", fieldName, _typeConstraint));
            }
         }

         if (isReadWrite || type == "var")
         {
            state.CreateYieldFlag();
            state.CreateReturnType();
            var _setter =
               from keyword in state.Scan(@"^(\s*)(set)\b", Color.Whitespace, Color.Keyword)
               from blockValue in getAnyBlock(state)
               select blockValue;
            if (_setter is (true, var setter))
            {
               state.RemoveYieldFlag();
               state.RemoveReturnType();
               var assign = new AssignReferenceToNewField(fieldName, "field");
               setter.Unshift(assign);
               state.AddStatement(new Function($"{name}=", new Parameters("value"), false, setter, false, false, ""));
            }
            else
            {
               state.AddStatement(Function.Setter($"{name}=", fieldName, _typeConstraint));
            }
         }

         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }
}