using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class CaseParser : StatementParser
{
   protected string assignmentField;
   protected string fieldName;
   protected bool mutable;
   protected bool assignment;
   protected bool top;
   protected CaseType caseType;

   public CaseParser(string assignmentField, bool mutable, bool assignment, string fieldName, bool top, CaseType caseType)
   {
      this.assignmentField = assignmentField;
      this.mutable = mutable;
      this.assignment = assignment;
      this.fieldName = fieldName;
      this.top = top;
      this.caseType = caseType;
   }

   public CaseParser(string fieldName)
   {
      assignmentField = "";
      mutable = false;
      assignment = false;
      this.fieldName = fieldName;
      top = false;

      caseType = CaseType.Function;
   }

   public override string Pattern => "^ /(/s*) /'case' /(/s*)";

   protected static Optional<Block> getCaseBlock(CaseType caseType, ParseState state) => caseType switch
   {
      CaseType.Statement => getCaseStatementBlock(state),
      CaseType.Function => getAnyBlock(state),
      CaseType.Lambda => getBlock(state),
      _ => fail($"Didn't understand case type {caseType}")
   };

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);

      var _result =
         from comparisandValue in getCompoundComparisands(state, fieldName)
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
            var caseParser = new CaseParser(assignmentField, mutable, assignment, fieldName, false, caseType);
            Maybe<If> _ifStatement = nil;
            var _scan = caseParser.Scan(state);
            if (_scan)
            {
               _ifStatement = caseParser.If;
            }
            else if (_scan.Exception is (true, var exception))
            {
               return exception;
            }

            If = new If(expression, block, _ifStatement, nil, assignmentField, mutable, assignment, top);
            return unit;
         }
         else
         {
            return _expression.Exception;
         }
      }
      else
      {
         return _result.Exception | expectedValue;
      }
   }

   public Maybe<If> If { get; set; } = nil;
}