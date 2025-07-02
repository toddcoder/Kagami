using System.Text.RegularExpressions;
using Core.Collections;
using Core.Matching;
using Core.Monads;
using Kagami.Library.Nodes;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class SimpleLambdaParser : SymbolParser
{
   public SimpleLambdaParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\.)(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();

      state.Colorize(tokens, Color.Whitespace, Color.Lambda, Color.Lambda);
      var _expression = getExpression(state, @"^(\))", builder.Flags & ~ExpressionFlags.Comparisand | ExpressionFlags.InLambda, Color.Lambda);
      if (_expression is (true, var expression))
      {
         StringSet parameterNames = [.. getParameterNames(expression)];
         var block = new Block(expression);
         var lambdaSymbol = new LambdaSymbol(parameterNames.Count, block);
         builder.Add(lambdaSymbol);

         state.CommitTransaction();

         return unit;
      }
      else
      {
         state.RollBackTransaction();
         return _expression.Exception;
      }

      IEnumerable<string> getParameterNames(Expression expression)
      {
         foreach (var symbol in expression.Symbols)
         {
            switch (symbol)
            {
               case FieldSymbol fieldSymbol when fieldSymbol.FieldName.IsMatch("^ '__$' /d+ /b"):
                  yield return fieldSymbol.FieldName;

                  break;
               case IHasExpression hasExpression:
               {
                  foreach (var parameterName in getParameterNames(hasExpression.Expression))
                  {
                     yield return parameterName;
                  }

                  break;
               }
               case IHasExpressions hasExpressions:
               {
                  foreach (var subExpression in hasExpressions.Expressions)
                  {
                     foreach (var parameterName in getParameterNames(subExpression))
                     {
                        yield return parameterName;
                     }
                  }

                  break;
               }
            }
         }
      }
   }
}