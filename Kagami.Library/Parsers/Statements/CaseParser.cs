using System;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class CaseParser : StatementParser
   {
      string assignmentField;
      string fieldName;
      bool mutable;
      bool assignment;
      bool top;
      CaseType caseType;

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

      static IMatched<Block> getCaseBlock(CaseType caseType, ParseState state)
      {
         switch (caseType)
         {
            case CaseType.Statement:
               return getCaseStatementBlock(state);
            case CaseType.Function:
	            return getAnyBlock(state);
            case CaseType.Lambda:
               return getBlock(state);
            default:
               throw new ArgumentOutOfRangeException(nameof(caseType), caseType, null);
         }
      }

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Structure, Color.Whitespace);

         var result =
            from comparisand in getExpression(state, ExpressionFlags.Comparisand | ExpressionFlags.OmitIf)
            from and in andExpression(state)
            from end in state.SkipEndOfLine().Or(() => "".Matched())
            from block in getCaseBlock(caseType, state)
            select (comparisand, and, block);

         if (result.If(out var tuple, out var original))
         {
            var (comparisand, and, block) = tuple;

            var builder = new ExpressionBuilder(ExpressionFlags.Standard);
            builder.Add(new FieldSymbol(fieldName));
            builder.Add(comparisand);
            //builder.Add(new SendMessageSymbol("match", Precedence.SendMessage, comparisand));
            builder.Add(new MatchSymbol());
            if (and.If(out var a))
               builder.Add(a);

            if (builder.ToExpression().If(out var expression, out var exception))
            {
               var caseParser = new CaseParser(assignmentField, mutable, assignment, fieldName, false, caseType);
               var (type, _, caseException) = caseParser.Scan(state).Values;
               var ifStatement = none<If>();
               switch (type)
               {
                  case MatchType.Matched:
                     ifStatement = caseParser.If.Some();
                     break;
                  case MatchType.NotMatched:
                     break;
                  case MatchType.FailedMatch:
                     return failedMatch<Unit>(caseException);
               }

               If = new If(expression, block, ifStatement, none<Block>(), assignmentField, mutable, assignment, top);
               return Unit.Matched();
            }
            else
               return failedMatch<Unit>(exception);
         }
         else if (original.IsNotMatched)
            return failedMatch<Unit>(expectedValue());
         else
            return original.Unmatched<Unit>();
      }

      public If If { get; set; }
   }
}