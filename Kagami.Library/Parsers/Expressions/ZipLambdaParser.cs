using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ZipLambdaParser : SymbolParser
{
   public ZipLambdaParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => "^ /(/s*) /'['";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);
      state.BeginTransaction();

      var expressionBuilder = new ExpressionBuilder(builder.Flags);
      var parser = new AnyLambdaParser(expressionBuilder);
      var _result =
         from parsed in parser.Scan(state)
         from endToken in state.Scan("/']'", Color.Operator)
         select parsed;

      if (_result)
      {
         var _expression = expressionBuilder.ToExpression();
         if (_expression is (true, var expression))
         {
            builder.Add(new ZipLambdaSymbol(expression));
            state.CommitTransaction();

            return unit;
         }
         else
         {
            state.RollBackTransaction();
            return _expression.Exception;
         }
      }
      else
      {
         state.RollBackTransaction();
         return _result.Exception;
      }
   }
}