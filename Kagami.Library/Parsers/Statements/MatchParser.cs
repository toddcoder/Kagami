using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class MatchParser : StatementParser
   {
      public override string Pattern => $"^ (/('var' | 'let') /(|s|) /({REGEX_FIELD}) /(|s|) /'=' /(|s|))? /'match' /(/s+)";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var mutable = tokens[1].Text == "var";
         var fieldName = tokens[3].Text;
         var assignment = fieldName.IsNotEmpty();

         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure, Color.Whitespace,
            Color.Keyword, Color.Whitespace);

         if (getExpression(state, ExpressionFlags.Standard).If(out var expression, out var original1))
         {
            var matchField = newLabel("match");
            state.AddStatement(new AssignToNewField(false, matchField, expression));

            state.Advance();
            var caseParser = new CaseParser(fieldName, mutable, assignment, matchField, true, CaseType.Statement);
            if (caseParser.Scan(state).If(out _, out var original2))
            {
               var ifStatement = caseParser.If;
               addMatchElse(ifStatement);
               state.AddStatement(ifStatement);
               state.Regress();

               return Unit.Matched();
            }
            else
            {
               state.Regress();
               return original2.Unmatched<Unit>();
            }
         }
         else
            return original1.Unmatched<Unit>();
      }
   }
}