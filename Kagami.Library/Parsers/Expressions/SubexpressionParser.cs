using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SubexpressionParser : SymbolParser
   {
      public SubexpressionParser(ExpressionBuilder builder) : base(builder)
      {
      }

      public override string Pattern => "^ /(|s|) /'(' /','?";

      public override Responding<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.BeginTransaction();
         var monoTuple = tokens[3].Text == ",";
         state.Colorize(tokens, Color.Whitespace, Color.OpenParenthesis, Color.Structure);

         var _expression = getExpression(state, "^ /')'", builder.Flags & ~ExpressionFlags.OmitComma, Color.CloseParenthesis);
         if (_expression)
         {
            builder.Add(new SubexpressionSymbol(_expression, monoTuple));
            state.CommitTransaction();

            return unit;
         }
         else if (_expression.AnyException)
         {
            state.RollBackTransaction();
            state.BeginTransaction();
            var _lambdaSymbol = getPartialLambda(state);
            if (_lambdaSymbol)
            {
               state.CommitTransaction();
               builder.Add(_lambdaSymbol);

               return unit;
            }
            else if (_lambdaSymbol.AnyException)
            {
               state.RollBackTransaction();
               return _lambdaSymbol.Exception;
            }
            else
            {
               state.RollBackTransaction();
               return nil;
            }
         }
         else
         {
            return nil;
         }
      }
   }
}