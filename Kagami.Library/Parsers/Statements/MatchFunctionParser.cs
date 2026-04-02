using System.Text.RegularExpressions;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class MatchFunctionParser : StatementParser
{
   [GeneratedRegex($@"^(\s*){REGEX_HIDDEN}(override\s+)?(match)(\s+)({REGEX_FUNCTION_NAME})(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var isHidden = tokens[2].Text.IsNotEmpty();
      var overriding = tokens[3].Text.StartsWith("override");
      var functionName = tokens[6].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Invokable, Color.OpenParenthesis);

      state.CreateYieldFlag();
      state.CreateReturnType();

      var _parameters = getParameters(state);
      if (_parameters is (true, var parameters))
      {
         List<If> list = [];
         Maybe<TypeConstraint> _typeConstraint = parseTypeConstraint(state).Map(ptc => ptc.Maybe);
         state.SetReturnType(_typeConstraint);

         var isFixed = (bool)state.Scan(@"^(\s+)(fixed)", Color.Whitespace, Color.Keyword);

         var _beginBlock = state.BeginBlock();
         if (_beginBlock)
         {
         }
         else
         {
            return _beginBlock.Exception;
         }

         var fieldName = newLabel("match");

         while (state.More)
         {
            var _endBlock = state.EndBlock();
            if (_endBlock)
            {
               break;
            }
            else if (_endBlock.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               var whenParser = new WhenParser(fieldName);
               var _scan = whenParser.Scan(state);
               if (_scan)
               {
                  if (whenParser.If is (true, var ifStatement))
                  {
                     ifStatement.AddReturnIf();
                     list.Add(ifStatement);
                  }
               }
               else if (_scan.Exception is (true, var scanException))
               {
                  return scanException;
               }
               else
               {
                  break;
               }
            }
         }

         if (list.Count == 0)
         {
            state.RemoveReturnType();
            return nil;
         }
         else
         {
            Stack<If> stack = [];
            foreach (var ifStatement in list)
            {
               stack.Push(ifStatement);
            }

            var previousIf = stack.Pop();
            while (stack.Count > 0)
            {
               var current = stack.Pop();
               current.ElseIf = previousIf;
               previousIf = current;
            }

            previousIf.Else = new Block(new FailedMatch());

            var block = new Block([new MatchFunctionAssignment(fieldName, parameters), previousIf, new ReturnNothing()]);
            var function = new Function(functionName, parameters, isHidden, block, false, overriding, ClassName | "")
            {
               SelfAlias = SelfAlias, IsFixed = isFixed
            };
            state.AddStatement(function);
            state.RemoveReturnType();

            return unit;
         }
      }
      else
      {
         return _parameters.Exception;
      }
   }

   public Maybe<string> ClassName { get; set; } = nil;

   public string SelfAlias { get; set; } = "";
}