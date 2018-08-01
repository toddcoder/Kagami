using System.Linq;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using Standard.Types.Numbers;
using static Standard.Types.Maybe.MaybeFunctions;

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
            return notMatched<Unit>();

         builder = new ExpressionBuilder(flags);
         prefixParser = new PrefixParser(builder);
         valuesParser = new ValuesParser(builder);
         infixParser = new InfixParser(builder);
         postfixParser = new PostfixParser(builder);
         conjunctionParsers = new ConjunctionParsers(builder);
         whateverCount = 0;

         if (getTerm(state).If(out _, out var isNotMatched, out var exception))
         {
            while (state.More)
            {
               if (!flags[ExpressionFlags.OmitConjunction])
               {
                  var conjunction = conjunctionParsers.Scan(state);
                  if (conjunction.IsMatched)
                     break;
                  else if (conjunction.IsFailedMatch)
                     return conjunction;
               }

               if (infixParser.Scan(state).If(out _, out isNotMatched, out exception))
               {
                  if (getTerm(state).If(out _, out isNotMatched, out exception))
                     continue;

                  if (isNotMatched)
                     break;

                  return failedMatch<Unit>(exception);
               }

               if (isNotMatched)
                  break;

               return failedMatch<Unit>(exception);
            }

            if (builder.ToExpression().If(out var expression, out exception))
            {
               if (state.MapExpression.If(out var mapExpression))
               {
                  var (fieldName, symbol) = mapExpression;
                  if (!keep(fieldName))
                  {
                     Expression = expression;
                     return Unit.Matched();
                  }
                  else if (getMessageWithLambda(fieldName, symbol, "map", expression).If(out var newExpression, out exception))
                  {
                     Expression = newExpression;
                     state.MapExpression = none<(string, Symbol)>();
                  }
                  else
                     return failedMatch<Unit>(exception);
               }
               else if (state.IfExpression.If(out var ifExpression))
               {
                  var (fieldName, symbol) = ifExpression;
                  if (!keep(fieldName))
                  {
                     Expression = expression;
                     return Unit.Matched();
                  }
                  else if (getMessageWithLambda(fieldName, symbol, "if", expression).If(out var newExpression, out exception))
                  {
                     Expression = newExpression;
                     state.IfExpression = none<(string, Symbol)>();
                  }
                  else
                     return failedMatch<Unit>(exception);
               }
               else if (state.LeftZipExpression.If(out var leftTuple) && state.RightZipExpression.If(out var rightTuple))
               {
                  var (leftFieldName, leftSymbol) = leftTuple;
                  var (rightFieldName, rightSymbol) = rightTuple;
                  if (!keep(leftFieldName) || !keep(rightFieldName))
                  {
                     Expression = expression;
                     return Unit.Matched();
                  }
                  else if (getDualMessageWithLambda(leftFieldName, rightFieldName, leftSymbol, rightSymbol, "zip".Function("", "with"),
                     expression).If(out var newExpression, out exception))
                  {
                     Expression = newExpression;
                     state.LeftZipExpression = none<(string, Symbol)>();
                     state.RightZipExpression = none<(string, Symbol)>();
                  }
                  else
                     return failedMatch<Unit>(exception);
               }
               else if (whateverCount > 0)
               {
                  var lambda = new LambdaSymbol(whateverCount,
                     new Block(new ExpressionStatement(expression, true)) { Index = expression.Index });
                  Expression = new Expression(lambda);
               }
               else
                  Expression = expression;

               return Unit.Matched();
            }
            else
               return failedMatch<Unit>(exception);
         }

         if (isNotMatched)
            return "Invalid expression syntax".FailedMatch<Unit>();
         else
            return failedMatch<Unit>(exception);
      }

/*      IMatched<(string, Symbol)> getIteratorExpression(IMaybe<(string, Symbol)> source, string message, Expression expression)
      {
         if (source.If(out var tuple))
         {
            var (fieldName, symbol) = tuple;
            if (keep(fieldName) && getMessageWithLambda(fieldName, symbol, message, expression)
               .If(out var newExpression, out var exception))
            {
               Expression = newExpression;
               return 
            }
         }
         else
            return notMatched<(string, Symbol)>();
      }*/

      bool keep(string fieldName)
      {
         var exp = builder.Ordered.ToArray();
         if (exp.Length != 1)
            return true;
         else
            return !(exp[0] is FieldSymbol fieldSymbol) || fieldSymbol.FieldName != fieldName;
      }

      static IResult<Expression> getMessageWithLambda(string fieldName, Symbol symbol, string messageName, Expression expression)
      {
         var parameter = Parameter.New(false, fieldName);
         var parameters = new Parameters(parameter);
         var lambdaSymbol = new LambdaSymbol(parameters, new Block(new Return(expression)));
         var sendMessage = new SendMessageSymbol(messageName, Precedence.PostfixOperator, lambdaSymbol.Some());

         var builder = new ExpressionBuilder(ExpressionFlags.Standard);
         builder.Add(symbol);
         builder.Add(sendMessage);

         return builder.ToExpression();
      }

      static IResult<Expression> getDualMessageWithLambda(string leftName, string rightName, Symbol leftSymbol, Symbol rightSymbol,
         string messageName, Expression expression)
      {
         var leftParameter = Parameter.New(false, leftName);
         var rightParameter = Parameter.New(false, rightName);
         var parameters = new Parameters(leftParameter, rightParameter);
         var lambdaSymbol = new LambdaSymbol(parameters, new Block(new Return(expression)));
         var sendMessage = new SendMessageSymbol(messageName, Precedence.PostfixOperator, lambdaSymbol.Some(),
            new Expression(rightSymbol));

         var builder = new ExpressionBuilder(ExpressionFlags.Standard);
         builder.Add(leftSymbol);
         builder.Add(sendMessage);

         return builder.ToExpression();
      }

      protected static IMatched<Unit> getOutfixOperator(ParseState state, Parser parser)
      {
         var matched = parser.Scan(state);
         if (matched.IsFailedMatch)
            return matched;
         else
         {
            while (matched.IsMatched)
            {
               matched = parser.Scan(state);
               if (matched.IsFailedMatch)
                  return matched;
            }

            if (matched.IsFailedMatch)
               return matched;
            else
               return notMatched<Unit>();
         }
      }

      protected IMatched<Unit> getValue(ParseState state)
      {
         if (valuesParser.Scan(state).If(out _, out var original))
         {
            if (builder.LastSymbol.If(out var lastSymbol) && lastSymbol is WhateverSymbol whatever)
               whatever.Count = whateverCount++;
            return Unit.Matched();
         }
         else if (original.IsFailedMatch)
            return valuesParser.Scan(state);
         else
            return "Invalid expression syntax".FailedMatch<Unit>();
      }

      protected IMatched<Unit> getTerm(ParseState state)
      {
         if ((getOutfixOperator(state, prefixParser).If(out _, out var original) || original.IsNotMatched) &&
            getValue(state).If(out _, out original) &&
            (getOutfixOperator(state, postfixParser).If(out _, out original) || original.IsNotMatched))
            return Unit.Matched();
         else
            return original;
      }
   }
}