using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class WhenParser : StatementParser
{
   protected string assignmentField;
   protected string fieldName;
   protected bool mutable;
   protected bool assignment;
   protected bool top;
   protected CaseType caseType;

   public WhenParser(string assignmentField, bool mutable, bool assignment, string fieldName, bool top, CaseType caseType)
   {
      this.assignmentField = assignmentField;
      this.mutable = mutable;
      this.assignment = assignment;
      this.fieldName = fieldName;
      this.top = top;
      this.caseType = caseType;
   }

   public WhenParser(string fieldName)
   {
      assignmentField = "";
      mutable = false;
      assignment = false;
      this.fieldName = fieldName;
      top = false;

      caseType = CaseType.Function;
   }

   [GeneratedRegex(@"^(\s*)")]
   public override partial Regex Regex();

   protected static Optional<Block> getCaseBlock(CaseType caseType, ParseState state)
   {
      if (caseType == CaseType.Function && !state.LookAhead(@"^(\s*)(return)"))
      {
         return state.SetException("Expected return keyword");
      }

      return caseType switch
      {
         CaseType.Statement => getCaseStatementBlock(state),
         CaseType.Function => getCaseReturnBlock(state),
         CaseType.Lambda => getBlock(state),
         _ => fail($"Didn't understand case type {caseType}")
      };
   }

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      if (state.LookAhead(@"^\s*\}"))
      {
         return nil;
      }

      state.Colorize(tokens, Color.Whitespace);

      var not = state.NotKeyword();

      var _result =
         from comparisandValue in getCompoundComparisands(state, fieldName, not)
         from andValue in andExpression(state)
         from blockValue in getCaseBlock(caseType, state)
         select (comparisandValue, andValue, blockValue);

      if (_result is (true, var (comparisand, possibleAnd, block)))
      {
         var builder = new ExpressionBuilder(ExpressionFlags.Standard);
         builder.Add(comparisand);
         if (possibleAnd.Maybe is (true, var and))
         {
            builder.Add(and);
         }

         var _expression = builder.ToExpression();
         if (_expression is (true, var expression))
         {
            var caseParser = new WhenParser(assignmentField, mutable, assignment, fieldName, false, caseType);
            Maybe<If> _ifStatement = nil;
            var _scan = caseParser.Scan(state);
            if (_scan)
            {
               _ifStatement = caseParser.If;
            }
            else if (_scan.Exception is (true, var exception))
            {
               return state.SetException(messageNoWhen("match"), exception);
            }

            If = new If(expression, block, _ifStatement, nil, assignmentField, mutable, assignment, top, true);
            return unit;
         }
         else
         {
            return state.SetException(messageImproperException("when"), _expression.Exception);
         }
      }
      else
      {
         return state.SetException(messageImproperWhen(), _result.Exception);
      }
   }

   public Maybe<If> If { get; set; } = nil;
}