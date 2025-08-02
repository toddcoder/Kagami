using System.Text.RegularExpressions;
using Core.Collections;
using Core.Monads;
using Kagami.Library.Nodes;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class WhateverLambdaParser : SymbolParser
{
   public WhateverLambdaParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\^\()")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Lambda);

      var _expression = getExpression(state, @"(\))", builder.Flags | ExpressionFlags.InLambda, Color.Lambda);
      if (_expression is (true, var expression))
      {
         List<string> parameters = [];
         GetParameterNames(expression, parameters);
         StringSet parameterNames = [.. parameters];
         var block = new Block(expression);
         var lambdaSymbol = new LambdaSymbol(parameterNames.Count, block);
         builder.Add(lambdaSymbol);
      }
      else
      {
         return fail("Expression not finished");
      }

      return unit;
   }

   public static void GetParameterNames(Expression expression, List<string> parameters)
   {
      var symbols = expression.Symbols;
      for (var i = 0; i < symbols.Length; i++)
      {
         var symbol = symbols[i];
         switch (symbol)
         {
            case AnySymbol or WhateverSymbol:
            {
               var fieldName = $"__${parameters.Count}";
               symbols[i] = new FieldSymbol(fieldName);
               parameters.Add(fieldName);
               break;
            }
            case IHasExpression hasExpression:
               GetParameterNames(hasExpression.Expression, parameters);
               break;
            case IHasExpressions hasExpressions:
            {
               foreach (var innerExpression in hasExpressions.Expressions)
               {
                  GetParameterNames(innerExpression, parameters);
               }

               break;
            }
         }
      }
   }
}