using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class BuilderParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(builder)(\s+)({REGEX_CLASS})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[4].Text;
      var hasParameters = tokens[5].Text == "(";
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);

      Module.Global.Value.ForwardReference(className);

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
      var builderState = new BuilderState("value", newLabel("failure"));
      var first = true;
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

         var builderMembersParser = new BuilderMembersParser(builderState, first);
         _scan = builderMembersParser.Scan(state);
         if (_scan)
         {
            first = false;
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

         var builder = new ClassBuilder(className, parameters, "", [], false, block);
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
         }
      }
      else
      {
         return _statements.Exception;
      }
   }
}