using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class LetFunctionParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(let)(\s+)({REGEX_FUNCTION_NAME})(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();

      var fieldName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Invokable, Color.OpenParenthesis);
      var _parametersType =
         from parametersValue in getParameters(state)
         from possibleTypConstraintValue in parseTypeConstraint(state)
         select (parametersValue, possibleTypConstraintValue);
      if (state.Scan(@"^(\s*)(=)", Color.Whitespace, Color.Structure) && _parametersType is (true, var (parameters, possibleTypeConstraint)) &&
          getSingleLine(state, possibleTypeConstraint.Maybe) is (true, var block))
      {
         var lambdaSymbol = new LambdaSymbol(parameters, block);
         var function = new AssignLambda(fieldName, lambdaSymbol);
         state.AddStatement(function);

         state.CommitTransaction();
         return unit;
      }
      else
      {
         state.CreateYieldFlag();
         state.CreateReturnType();

         var _block = getAnyBlock(state);
         if (_block is (true, var block2))
         {
            state.RemoveYieldFlag();
            state.RemoveReturnType();
            block2.AddReturnUnitIf();

            var lambdaSymbol = new LambdaSymbol(new Parameters(), block2);
            var function = new AssignLambda(fieldName, lambdaSymbol);
            state.AddStatement(function);

            state.CommitTransaction();
            return unit;
         }
         else
         {
            state.RollBackTransaction();
            return _block.Exception;
         }
      }
   }
}