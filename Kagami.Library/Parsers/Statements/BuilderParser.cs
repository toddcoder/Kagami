using Core.Monads;
using Kagami.Library.Invokables;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Failure = Kagami.Library.Objects.Failure;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class BuilderParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(builder)(\s+)({REGEX_FUNCTION_NAME})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var builderName = tokens[4].Text;
      var hasParameters = tokens[5].Text == "(";
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);

      //Module.Global.Value.ForwardReference(builderName);

      Parameters parameters;

      if (hasParameters)
      {
         var _parameters = getParameters(state);
         if (_parameters)
         {
            parameters = _parameters;
         }
         else if (_parameters.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            parameters = [with(0)];
         }
      }
      else
      {
         parameters = Parameters.Empty;
      }

      var defaultExpression = new Expression(new PushObjectSymbol(new Failure("No value")));
      var builderState = new BuilderState("value", newLabel("failure"), true);
      state.PushStatements();

      var _scan = state.BeginBlock();
      if (!_scan)
      {
         return _scan.Exception;
      }

      while (state.More)
      {
         _scan = state.EndBlock();
         if (_scan)
         {
            break;
         }
         else if (_scan.Exception is (true, var exception))
         {
            return exception;
         }

         var builderMembersParser = new BuilderMembersParser(builderState);
         _scan = builderMembersParser.Scan(state);
         if (_scan)
         {
            if (builderState.First)
            {
               builderState = builderState with { First = false };
            }
         }
         else
         {
            return _scan.Exception;
         }
      }

      var _statements = state.PopStatements();
      if (_statements is (true, var statements))
      {
         Block block = [new AssignToNewField(true, builderState.ResultFieldName, defaultExpression, false, false)];
         foreach (var statement in statements)
         {
            block.Add(statement);
            if (statement is BuilderReturn)
            {
               break;
            }
         }

         var fieldSymbol = new FieldSymbol(builderState.ResultFieldName);
         block.Add(new ExpressionStatement(fieldSymbol, true));

         var function = new Function(builderName, parameters, false, block, false, false, "");
         state.AddStatement(function);

         return unit;

         /*var builder = new ClassBuilder(builderName, parameters, "", [], false, block);
         var _register = builder.Register();
         if (_register)
         {
            var cls = new Class(builder);
            state.AddStatement(cls);

            return unit;
         }
         else
         {
            return _register.Exception;
         }*/
      }
      else
      {
         return _statements.Exception;
      }
   }
}