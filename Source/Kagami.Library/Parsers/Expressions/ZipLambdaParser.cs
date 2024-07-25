using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class ZipLambdaParser : SymbolParser
   {
      public ZipLambdaParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'['";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator);
         state.BeginTransaction();

         var expressionBuilder = new ExpressionBuilder(builder.Flags);
         var parser = new AnyLambdaParser(expressionBuilder);
         var result =
            from parsed in parser.Scan(state)
            from endToken in state.Scan("/']'", Color.Operator)
            select parsed;

         if (result.ValueOrOriginal(out _, out var original))
         {
            if (expressionBuilder.ToExpression().If(out var expression, out var exception))
            {
               builder.Add(new ZipLambdaSymbol(expression));
               state.CommitTransaction();

               return Unit.Matched();
            }
            else
            {
               state.RollBackTransaction();
               return failedMatch<Unit>(exception);
            }
         }
         else
         {
            state.RollBackTransaction();
            return original;
         }
      }
   }
}