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
               if (whateverCount > 0)
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