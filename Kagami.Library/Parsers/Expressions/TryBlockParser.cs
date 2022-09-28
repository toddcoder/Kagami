using System;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class TryBlockParser : SymbolParser
   {
      public TryBlockParser(ExpressionBuilder builder) : base(builder)
      {
      }

      public override string Pattern => $"^ /(|s|) /'try' /({REGEX_EOL})";

      public override Responding<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);

         var _block = getBlock(state);
         if (_block)
         {
            _block.Value.AddReturnIf(new UnitSymbol());
            var lambda = new LambdaSymbol(0, _block);
            var invokeBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
            invokeBuilder.Add(lambda);
            invokeBuilder.Add(new PostfixInvokeSymbol(Array.Empty<Expression>()));
            var _invokeExpression = invokeBuilder.ToExpression();
            if (_invokeExpression)
            {
               builder.Add(new TrySymbol(_invokeExpression));
               return unit;
            }
            else
            {
               return _invokeExpression.Exception;
            }
         }
         else
         {
            return _block.Map(_ => unit);
         }
      }
   }
}