using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ReductionParser : SymbolParser
{
   public override string Pattern => "^ /(|s+|) /'['";

   public ReductionParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();
      state.Colorize(tokens, Color.Whitespace, Color.Operator);

      var innerBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
      innerBuilder.Add(new FieldSymbol("__$0"));
      var operatorsParser = new OperatorsParser(innerBuilder);

      var _result =
         from op in operatorsParser.Scan(state)
         from closing in state.Scan("^ /']'", Color.Operator)
         select op;

      if (_result)
      {
         innerBuilder.Add(new FieldSymbol("__$1"));
         var _expression = innerBuilder.ToExpression();
         if (_expression is (true, var expression))
         {
            var lambda = new LambdaSymbol(2, expression);
            builder.Add(new SendBinaryMessageSymbol("foldr", Precedence.ChainedOperator));
            builder.Add(lambda);
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