using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class BlockLambdaParser : SymbolParser
{
   public BlockLambdaParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\{)(\||\()")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.CreateYieldFlag();
      state.CreateReturnType();

      var standardParameters = tokens[3].Text == "(";
      if (standardParameters)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Lambda, Color.OpenParenthesis);
         var _result =
            from parameters in getParameters(state)
            from possibleTypeConstraint in parseTypeConstraint(state)
            from block in getPartialBlock(state, possibleTypeConstraint.Maybe, Color.Lambda)
            select new LambdaSymbol(parameters, block, true);
         if (_result is (true, var lambdaSymbol))
         {
            builder.Add(lambdaSymbol);
            return unit;
         }
         else
         {
            return _result.Exception;
         }
      }
      else
      {
         state.Colorize(tokens, Color.Whitespace, Color.Lambda, Color.Operator);
         var _result =
            from c in getExpression(state, builder.Flags | ExpressionFlags.Comparisand)
            from possibleTypeConstraint in parseTypeConstraint(state)
            from blockValue in getPartialBlock(state, possibleTypeConstraint.Maybe, Color.Lambda)
            select (c, blockValue);
         if (_result is (true, var (comparisand, block)))
         {
            var expressionBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
            expressionBuilder.Add(new FieldSymbol("__$0"));
            expressionBuilder.Add(new MatchSymbol(false));
            expressionBuilder.Add(comparisand);

            var _comparison = expressionBuilder.ToExpression();
            if (_comparison is (true, var comparison))
            {
               var list = new List<Statement> { new If(comparison, block) };
               var lambdaSymbol = new LambdaSymbol(1, new Block(list));
               builder.Add(lambdaSymbol);

               return unit;
            }
            else
            {
               return _comparison.Exception;
            }
         }
         else
         {
            return _result.Exception;
         }
      }
   }
}