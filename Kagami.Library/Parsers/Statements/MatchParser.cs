using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using Core.Strings;
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
         var matchField = newLabel("match");
         state.AddStatement(new AssignToNewField(false, matchField, expression));

         var _result = state.BeginBlock();
         if (!_result)
         {
            return _result.Exception;
         }

         var caseParser = new CaseParser(fieldName, mutable, assignment, matchField, true, CaseType.Statement);
         var _scan = caseParser.Scan(state);
         if (_scan)
         {
            var ifStatement = caseParser.If;
            addMatchElse(ifStatement);
            state.AddStatement(ifStatement);

            return state.EndBlock();
         }
         else
         {
            return _scan.Exception;
         }
      }
      else
      {
         return _expression.Exception;
      }
   }
}