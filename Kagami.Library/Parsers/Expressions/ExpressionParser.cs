using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ExpressionParser : PatternlessParser
{
   protected Bits32<ExpressionFlags> flags;
   protected ExpressionBuilder builder;
   protected PrefixParser prefixParser;
   protected ValuesParser valuesParser;
   protected InfixParser infixParser;
   protected PostfixParser postfixParser;
   protected ConjunctionParsers conjunctionParsers;
   //protected EndOfLineParser endOfLineParser = new();
   protected int whateverCount;

   public ExpressionParser(Bits32<ExpressionFlags> flags) : base(false)
   {
      this.flags = flags;

      builder = new ExpressionBuilder(flags);
      prefixParser = new PrefixParser(builder);
      valuesParser = new ValuesParser(builder);
      infixParser = new InfixParser(builder);
      postfixParser = new PostfixParser(builder);
      conjunctionParsers = new ConjunctionParsers(builder);
   }

   public Expression Expression { get; set; } = Expression.Empty;

   protected static bool endOfLine(ParseState state)
   {
      var current = state.CurrentSource;
      return current.IsEmpty() || current.IsMatch("^ /s* '{'") || current.IsMatch("^ ((/r /n)+ | /r+ | /n+ |$)");
   }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens)
   {
      if (!state.More)
      {
         return nil;
      }

      whateverCount = 0;

      state.BeginPrefixCode();
      state.BeginImplicitState();

      try
      {
         var _term0 = getTerm(state);
         if (_term0)
         {
            /*
            var _anticipated = anticipateBrackets(state);
            var notEndOfLine = true;
            if (_anticipated)
            {
               notEndOfLine = false;
            }
            else if (_anticipated.Exception is (true, var anticipatedException))
            {
               return anticipatedException;
            }
            */

            var isEndOfLine = endOfLine(state);
            /*var _endOfLine = endOfLineParser.Scan(state);
            if (_endOfLine)
            {
               notEndOfLine = false;
            }
            else if (_endOfLine.Exception is (true, var endOfLineException))
            {
               return endOfLineException;
            }*/

            while (!isEndOfLine && state.More)
            {
               if (!flags[ExpressionFlags.OmitConjunction])
               {
                  var _conjunction = conjunctionParsers.Scan(state);
                  if (_conjunction)
                  {
                     break;
                  }
                  else if (_conjunction.Exception)
                  {
                     return _conjunction.Exception;
                  }
               }

               var _infix = infixParser.Scan(state);
               if (_infix)
               {
                  var _term1 = getTerm(state);
                  if (_term1)
                  {
                     /*_anticipated = anticipateBrackets(state);
                     if (_anticipated)
                     {
                        notEndOfLine = false;
                     }
                     else if (_anticipated.Exception is (true, var anticipatedException))
                     {
                        return anticipatedException;
                     }*/

                     isEndOfLine = endOfLine(state);
                     /*_endOfLine = endOfLineParser.Scan(state);
                     if (_endOfLine)
                     {
                        notEndOfLine = false;
                     }
                     else if (_endOfLine.Exception is (true, var endOfLineException))
                     {
                        return endOfLineException;
                     }*/
                  }
                  else if (_term1.Exception is (true, var exception))
                  {
                     return exception;
                  }
                  else
                  {
                     break;
                  }
               }
               else if (_infix.Exception is (true, var exception))
               {
                  return exception;
               }
               else
               {
                  break;
               }
            }

            var _expression = builder.ToExpression();
            if (_expression is (true, var expression))
            {
               var _implicitState = state.ImplicitState;
               if (_implicitState is (true, var implicitState) && !implicitState.Two)
               {
                  var _messageWithLambda =
                     getMessageWithLambda(implicitState.Symbol, implicitState.Message, implicitState.ParameterCount, expression);
                  if (_messageWithLambda is (true, var messageWithLambda))
                  {
                     Expression = messageWithLambda;
                     state.ImplicitState = nil;
                  }
                  else
                  {
                     return _messageWithLambda.Exception;
                  }
               }
               else if (state.ImplicitState is (true, { Two: (true, var symbol) } implicitState2))
               {
                  if (getDualMessageWithLambda("__$0", "__$1", implicitState2.Symbol, symbol, implicitState2.Message, expression) is
                      (true, var newExpression))
                  {
                     Expression = newExpression;
                     state.ImplicitState = nil;
                  }
               }
               else if (whateverCount > 0)
               {
                  var block = new Block(new ExpressionStatement(expression, true)) { Index = expression.Index };
                  var lambda = new LambdaSymbol(whateverCount, block);
                  Expression = new Expression(lambda);
               }
               else
               {
                  Expression = expression;
               }

               return unit;
            }
            else
            {
               return _expression.Exception;
            }
         }
         else if (_term0.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            return fail("Invalid expression syntax");
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

   protected static Result<Expression> getMessageWithLambda(Symbol symbol, Selector selector, int parameterCount, Expression expression)
   {
      var parameters = new Parameters(Enumerable.Range(0, parameterCount).Select(i => $"__${i}").ToArray());
      var lambdaSymbol = new LambdaSymbol(parameters, new Block(new Return(expression, nil)));
      var sendMessage = new SendMessageSymbol(selector, lambdaSymbol.Some());

      var builder = new ExpressionBuilder(ExpressionFlags.Standard);
      builder.Add(symbol);
      builder.Add(sendMessage);

      return builder.ToExpression();
   }

   protected static Result<Expression> getMessage2WithLambda(string leftName, string rightName, Symbol symbol, Selector selector,
      Expression expression)
   {
      var leftParameter = Parameter.New(false, leftName);
      var rightParameter = Parameter.New(false, rightName);
      var parameters = new Parameters(leftParameter, rightParameter);
      var lambdaSymbol = new LambdaSymbol(parameters, new Block(new Return(expression, nil)));
      var sendMessage = new SendMessageSymbol(selector, lambdaSymbol.Some());

      var builder = new ExpressionBuilder(ExpressionFlags.Standard);
      builder.Add(symbol);
      builder.Add(sendMessage);

      return builder.ToExpression();
   }

   protected static Result<Expression> getDualMessageWithLambda(string leftName, string rightName, Symbol leftSymbol, Symbol rightSymbol,
      Selector selector, Expression expression)
   {
      var leftParameter = Parameter.New(false, leftName);
      var rightParameter = Parameter.New(false, rightName);
      var parameters = new Parameters(leftParameter, rightParameter);
      var lambdaSymbol = new LambdaSymbol(parameters, new Block(new Return(expression, nil)));
      var sendMessage = new SendMessageSymbol(selector, lambdaSymbol.Some(), new Expression(rightSymbol));

      var builder = new ExpressionBuilder(ExpressionFlags.Standard);
      builder.Add(leftSymbol);
      builder.Add(sendMessage);

      return builder.ToExpression();
   }

   protected static Optional<Unit> getOutfixOperator(ParseState state, Parser parser)
   {
      var _matched = parser.Scan(state);
      if (_matched.Exception)
      {
         return _matched.Exception;
      }
      else
      {
         while (_matched)
         {
            _matched = parser.Scan(state);
            if (_matched.Exception)
            {
               return _matched.Exception;
            }
         }

         if (_matched.Exception)
         {
            return _matched.Exception;
         }
         else
         {
            return nil;
         }
      }
   }

   protected Optional<Unit> getValue(ParseState state)
   {
      var _scan = valuesParser.Scan(state);
      if (_scan)
      {
         if (builder.LastSymbol is (true, WhateverSymbol whatever))
         {
            whatever.Count = whateverCount++;
         }

         return unit;
      }
      else if (_scan.Exception)
      {
         return valuesParser.Scan(state);
      }
      else
      {
         return fail("Invalid expression syntax");
      }
   }

   protected Optional<Unit> getTerm(ParseState state)
   {
      return
         from outfix1 in notExceptional(getOutfixOperator(state, prefixParser))
         from value in getValue(state)
         from outfix2 in notExceptional(getOutfixOperator(state, postfixParser))
         select unit;

      Optional<Unit> notExceptional(Optional<Unit> _original)
      {
         if (_original)
         {
            return unit;
         }
         else if (_original.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            return unit;
         }
      }
   }
}