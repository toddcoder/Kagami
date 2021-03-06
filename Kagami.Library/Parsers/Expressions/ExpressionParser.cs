﻿using Core.Monads;
using Core.Numbers;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using System.Linq;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class ExpressionParser : Parser
   {
      protected Bits32<ExpressionFlags> flags;
      protected ExpressionBuilder builder;
      protected PrefixParser prefixParser;
      protected ValuesParser valuesParser;
      protected InfixParser infixParser;
      protected PostfixParser postfixParser;
      protected ConjunctionParsers conjunctionParsers;
      protected int whateverCount;

      public ExpressionParser(Bits32<ExpressionFlags> flags) : base(false) => this.flags = flags;

      public Expression Expression { get; set; }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens)
      {
         if (!state.More)
         {
            return notMatched<Unit>();
         }

         builder = new ExpressionBuilder(flags);
         prefixParser = new PrefixParser(builder);
         valuesParser = new ValuesParser(builder);
         infixParser = new InfixParser(builder);
         postfixParser = new PostfixParser(builder);
         conjunctionParsers = new ConjunctionParsers(builder);
         whateverCount = 0;

         state.BeginPrefixCode();
         state.BeginImplicitState();

         try
         {
            if (getTerm(state).If(out _, out var anyException))
            {
               while (state.More)
               {
                  if (!flags[ExpressionFlags.OmitConjunction])
                  {
                     var conjunction = conjunctionParsers.Scan(state);
                     if (conjunction.IsMatched)
                     {
                        break;
                     }
                     else if (conjunction.IsFailedMatch)
                     {
                        return conjunction;
                     }
                  }

                  if (infixParser.Scan(state).If(out _, out anyException))
                  {
                     if (getTerm(state).If(out _, out anyException))
                     {
                     }
                     else if (anyException.If(out var exception))
                     {
                        return failedMatch<Unit>(exception);
                     }
                     else
                     {
                        break;
                     }
                  }
                  else if (anyException.If(out var exception))
                  {
                     return failedMatch<Unit>(exception);
                  }
                  else
                  {
                     break;
                  }
               }

               if (builder.ToExpression().If(out var expression, out var expException))
               {
                  if (state.ImplicitState.If(out var implicitState) && implicitState.Two.IsNone)
                  {
                     if (getMessageWithLambda(implicitState.Symbol, implicitState.Message, implicitState.ParameterCount, expression)
                        .If(out var newExpression, out expException))
                     {
                        Expression = newExpression;
                        state.ImplicitState = none<ImplicitState>();
                     }
                     else
                     {
                        return failedMatch<Unit>(expException);
                     }
                  }
                  else if (state.ImplicitState.If(out implicitState) && implicitState.Two.If(out var symbol))
                  {
                     if (getDualMessageWithLambda("__$0", "__$1", implicitState.Symbol, symbol, implicitState.Message, expression)
                        .If(out var newExpression, out _))
                     {
                        Expression = newExpression;
                        state.ImplicitState = none<ImplicitState>();
                     }
                  }
                  else if (whateverCount > 0)
                  {
                     var lambda = new LambdaSymbol(whateverCount,
                        new Block(new ExpressionStatement(expression, true)) { Index = expression.Index });
                     Expression = new Expression(lambda);
                  }
                  else
                  {
                     Expression = expression;
                  }

                  return Unit.Matched();
               }
               else
               {
                  return failedMatch<Unit>(expException);
               }
            }
            else if (anyException.If(out var exception))
            {
               return failedMatch<Unit>(exception);
            }
            else
            {
               return "Invalid expression syntax".FailedMatch<Unit>();
            }
         }
         finally
         {
            state.EndPrefixCode();
            state.EndImplicitState();
         }
      }

      protected bool keep(string fieldName)
      {
         var exp = builder.Ordered.ToArray();
         if (exp.Length != 1)
         {
            return true;
         }
         else
         {
            return exp[0] is not FieldSymbol fieldSymbol || fieldSymbol.FieldName != fieldName;
         }
      }

      protected static IResult<Expression> getMessageWithLambda(Symbol symbol, Selector selector, int parameterCount, Expression expression)
      {
         var parameters = new Parameters(Enumerable.Range(0, parameterCount).Select(i => $"__${i}").ToArray());
         var lambdaSymbol = new LambdaSymbol(parameters, new Block(new Return(expression, none<TypeConstraint>())));
         var sendMessage = new SendMessageSymbol(selector, lambdaSymbol.Some());

         var builder = new ExpressionBuilder(ExpressionFlags.Standard);
         builder.Add(symbol);
         builder.Add(sendMessage);

         return builder.ToExpression();
      }

      protected static IResult<Expression> getMessage2WithLambda(string leftName, string rightName, Symbol symbol, Selector selector,
         Expression expression)
      {
         var leftParameter = Parameter.New(false, leftName);
         var rightParameter = Parameter.New(false, rightName);
         var parameters = new Parameters(leftParameter, rightParameter);
         var lambdaSymbol = new LambdaSymbol(parameters, new Block(new Return(expression, none<TypeConstraint>())));
         var sendMessage = new SendMessageSymbol(selector, lambdaSymbol.Some());

         var builder = new ExpressionBuilder(ExpressionFlags.Standard);
         builder.Add(symbol);
         builder.Add(sendMessage);

         return builder.ToExpression();
      }

      protected static IResult<Expression> getDualMessageWithLambda(string leftName, string rightName, Symbol leftSymbol, Symbol rightSymbol,
         Selector selector, Expression expression)
      {
         var leftParameter = Parameter.New(false, leftName);
         var rightParameter = Parameter.New(false, rightName);
         var parameters = new Parameters(leftParameter, rightParameter);
         var lambdaSymbol = new LambdaSymbol(parameters, new Block(new Return(expression, none<TypeConstraint>())));
         var sendMessage = new SendMessageSymbol(selector, lambdaSymbol.Some(), new Expression(rightSymbol));

         var builder = new ExpressionBuilder(ExpressionFlags.Standard);
         builder.Add(leftSymbol);
         builder.Add(sendMessage);

         return builder.ToExpression();
      }

      protected static IMatched<Unit> getOutfixOperator(ParseState state, Parser parser)
      {
         var matched = parser.Scan(state);
         if (matched.IsFailedMatch)
         {
            return matched;
         }
         else
         {
            while (matched.IsMatched)
            {
               matched = parser.Scan(state);
               if (matched.IsFailedMatch)
               {
                  return matched;
               }
            }

            if (matched.IsFailedMatch)
            {
               return matched;
            }
            else
            {
               return notMatched<Unit>();
            }
         }
      }

      protected IMatched<Unit> getValue(ParseState state)
      {
         if (valuesParser.Scan(state).ValueOrOriginal(out _, out var original))
         {
            if (builder.LastSymbol.If(out var lastSymbol) && lastSymbol is WhateverSymbol whatever)
            {
               whatever.Count = whateverCount++;
            }

            return Unit.Matched();
         }
         else if (original.IsFailedMatch)
         {
            return valuesParser.Scan(state);
         }
         else
         {
            return "Invalid expression syntax".FailedMatch<Unit>();
         }
      }

      protected IMatched<Unit> getTerm(ParseState state)
      {
         if ((getOutfixOperator(state, prefixParser).ValueOrOriginal(out _, out var original) || original.IsNotMatched) &&
            getValue(state).ValueOrOriginal(out _, out original) &&
            (getOutfixOperator(state, postfixParser).ValueOrOriginal(out _, out original) || original.IsNotMatched))
         {
            return Unit.Matched();
         }
         else
         {
            return original;
         }
      }
   }
}