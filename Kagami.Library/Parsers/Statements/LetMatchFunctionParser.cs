using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class LetMatchFunctionParser : StatementParser
{
   protected static Optional<AssignLambda> getAssignLambda(Block block, Expression comparisand, string fieldName)
   {
      var expressionBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
      expressionBuilder.Add(new FieldSymbol("__$0"));
      expressionBuilder.Add(new MatchSymbol(false));
      expressionBuilder.Add(comparisand);

      var _comparison = expressionBuilder.ToExpression();
      if (_comparison is (true, var comparison))
      {
         List<Statement> list = [new If(comparison, block)];
         var lambdaSymbol = new LambdaSymbol(1, [with(list)]);
         return new AssignLambda(fieldName, lambdaSymbol);
      }
      else
      {
         return _comparison.Exception;
      }
   }

   [GeneratedRegex(@$"^(\s*)(let\s+match)(\s+)({REGEX_FUNCTION_NAME})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();

      var fieldName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Invokable);

      var _result =
         from comparisandValue in getExpression(state, ExpressionFlags.Standard | ExpressionFlags.Comparisand)
         from possibleTypeConstraintValue in parseTypeConstraint(state)
         select (comparisandValue, possibleTypeConstraintValue);
      if (_result is (true, var (comparisand, possibleTypeConstraint)))
      {
         if (state.Scan(@"^(\s*)(=)", Color.Whitespace, Color.Structure))
         {
            var _block = getSingleLine(state, possibleTypeConstraint.Maybe);
            if (_block is (true, var block))
            {
               var _assignLambda = getAssignLambda(block, comparisand, fieldName);
               if (_assignLambda is (true, var assignLambda))
               {
                  state.AddStatement(assignLambda);
                  state.CommitTransaction();
                  return unit;
               }
               else
               {
                  state.RollBackTransaction();
                  return _assignLambda.Exception;
               }
            }
            else
            {
               state.RollBackTransaction();
               return _block.Exception;
            }
         }
         else
         {
            state.CreateYieldFlag();
            state.CreateReturnType();

            var _block = getAnyBlock(state);
            if (_block is (true, var block))
            {
               state.RemoveYieldFlag();
               state.RemoveReturnType();
               block.AddReturnUnitIf();

               var _assignLambda = getAssignLambda(block, comparisand, fieldName);
               if (_assignLambda is (true, var assignLambda))
               {
                  state.AddStatement(assignLambda);
                  state.CommitTransaction();
                  return unit;
               }
               else
               {
                  state.RollBackTransaction();
                  return _assignLambda.Exception;
               }
            }
            else
            {
               state.RollBackTransaction();
               return _block.Exception;
            }
         }
      }
      else
      {
         return _result.Exception;
      }
   }
}