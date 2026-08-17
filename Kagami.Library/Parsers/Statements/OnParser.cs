using Core.Matching;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using System.Text.RegularExpressions;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class OnParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*){REGEX_HIDDEN}(fn)(\s+)({REGEX_FUNCTION_NAME})(?=\s*\|)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var isHidden = tokens[1].Text.IsNotEmpty();
      var functionName = tokens[5].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Invokable);

      var pattern = @$"^(\s*)(fn)(\s+)({functionName.Escape()})(?=\s*\|)";

      List<If> list = [];

      Maybe<TypeConstraint> _typeConstraint = parseTypeConstraint(state).Map(ptc => ptc.Maybe);

      state.CreateReturnType();
      while (state.More)
      {
         var caseParser = new WhenParser("__$0", CaseType.Arrow);
         state.SkipEndOfLine();
         var _scan = caseParser.Scan(state);
         if (_scan)
         {
            if (caseParser.If is (true, var @if))
            {
               list.Add(@if);
            }

            var _next = state.Scan(pattern, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Invokable);
            if (_next)
            {
            }
            else if (_next.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               break;
            }
         }
         else if (_scan.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      if (list.Count == 0)
      {
         state.RemoveReturnType();

         return nil;
      }
      else
      {
         var stack = new Stack<If>();
         foreach (var ifStatement in list)
         {
            stack.Push(ifStatement);
         }

         var previousIf = stack.Pop();
         while (stack.Count > 0)
         {
            var current = stack.Pop();
            current.ElseIf = previousIf.Some();
            previousIf = current;
         }

         previousIf.Else = new Block(new FailedMatch());

         var parameterName = "__$0";
         var variadicParameter = new Parameter(false, false, "", parameterName, nil, nil, false, false, false)
         {
            Variadic = true,
            Singleton = true
         };
         var parameters = new Parameters(variadicParameter);
         state.AddStatement(new MatchFunction(functionName, parameters, isHidden, previousIf, _typeConstraint, false, ""));
         state.RemoveReturnType();

         return unit;
      }
   }
}