using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class MatchFunctionCaseParser : StatementParser
   {
      string parameterName;

      public MatchFunctionCaseParser(string parameterName) => this.parameterName = parameterName;

      public override string Pattern => "^ /'|' /(|s|)";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Structure, Color.Whitespace);

         var result =
            from comparisand in getExpression(state, ExpressionFlags.Comparisand)
            from and in andExpression(state)
            from end in state.SkipEndOfLine()
            from block in getAnyBlock(state)
            select (comparisand, and, block);

         if (result.If(out var tuple, out var original))
         {
            (var comparisand, var and, var block) = tuple;

            var builder = new ExpressionBuilder(ExpressionFlags.Standard);
            builder.Add(new FieldSymbol(parameterName));
            builder.Add(new SendMessageSymbol("match", comparisand));
            if (and.If(out var a))
               builder.Add(a);

            if (builder.ToExpression().If(out var expression, out var exception))
            {
               var caseParser = new MatchFunctionCaseParser(parameterName);
               (var type, _, var caseException) = caseParser.Scan(state).Values;
               switch (type)
               {
                  case MatchType.Matched:
                     If = caseParser.If;
                     break;
                  case MatchType.NotMatched:
                     break;
                  case MatchType.FailedMatch:
                     return failedMatch<Unit>(caseException);
               }

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