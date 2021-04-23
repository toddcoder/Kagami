using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
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

      public override string Pattern => "^ /'|' /(|s|)";

      protected static IMatched<Block> getCaseBlock(CaseType caseType, ParseState state) => caseType switch
      {
         CaseType.Statement => getCaseStatementBlock(state),
         CaseType.Function => getAnyBlock(state),
         CaseType.Lambda => getBlock(state),
         _ => $"Didn't understand case type {caseType}".FailedMatch<Block>()
      };

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Structure, Color.Whitespace);

         var result =
            from comparisand in getCompoundComparisands(state, fieldName)
            from and in andExpression(state)
            from end in state.SkipEndOfLine().Or(() => "".Matched())
            from block in getCaseBlock(caseType, state)
            select (comparisand, and, block);

         if (result.ValueOrCast<Unit>(out var tuple, out var asUnit))
         {
            var (comparisand, _and, block) = tuple;

            var builder = new ExpressionBuilder(ExpressionFlags.Standard);
            builder.Add(comparisand);
            if (_and.If(out var and))
            {
               builder.Add(and);
            }

            if (builder.ToExpression().If(out var expression, out var exception))
            {
               var caseParser = new CaseParser(assignmentField, mutable, assignment, fieldName, false, caseType);
               var ifStatement = none<If>();
               if (caseParser.Scan(state).If(out _, out var _caseException))
               {
                  ifStatement = caseParser.If.Some();
               }
               else if (_caseException.If(out var caseException))
               {
                  return failedMatch<Unit>(caseException);
               }

               If = new If(expression, block, ifStatement, none<Block>(), assignmentField, mutable, assignment, top);
               return Unit.Matched();
            }
            else
            {
               return failedMatch<Unit>(exception);
            }
         }
         else if (asUnit.IsNotMatched)
         {
            return failedMatch<Unit>(expectedValue());
         }
         else
         {
            return asUnit;
         }
      }

      public If If { get; set; }
   }
}