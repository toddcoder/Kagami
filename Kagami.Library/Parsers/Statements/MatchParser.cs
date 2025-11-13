using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class MatchParser : StatementParser
{
   [GeneratedRegex($@"^(\s*)(?:(var|let)(\s*)({REGEX_FIELD})(\s*)(=)(\s*))?(match)(\s+)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var mutable = tokens[2].Text == "var";
      var fieldName = tokens[4].Text;
      var assignment = fieldName.IsNotEmpty();

      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure, Color.Whitespace,
         Color.Keyword, Color.Whitespace);

      var _expression = getExpression(state, ExpressionFlags.Standard);
      if (_expression is (true, var expression))
      {
         var _result = state.BeginBlock();
         if (!_result)
         {
            return state.SetException(messageNoBeginBlock("match"), _result.Exception);
         }

         if (assignment)
         {
            state.AddStatement(new NewFieldStatement(fieldName, mutable, nil));
         }

         state.AddStatement(new PushFrameStatement());
         var matchField = newLabel("match");
         state.AddStatement(new AssignToNewField(true, matchField, expression, false));

         var whenParser = new WhenParser(fieldName, mutable, assignment, matchField, false, CaseType.Statement);
         var _scan = whenParser.Scan(state);
         if (_scan)
         {
            var ifStatement = whenParser.If;
            addMatchElse(ifStatement);
            state.AddStatement(ifStatement);

            state.AddStatement(new PopFrameStatement());
            return state.EndBlock();
         }
         else
         {
            return state.SetException(messageNoWhen("match"));
         }
      }
      else
      {
         return state.SetException(messageMatchValue(), _expression.Exception);
      }
   }
}