using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ConditionalAssignParser : StatementParser
   {
      public override string Pattern => "^ /'if' /(|s+|)";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.BeginTransaction();

         state.Colorize(tokens, Color.Keyword, Color.Whitespace);

         var result =
            from comparisand in getExpression(state, ExpressionFlags.Comparisand)
            from scanned in state.Scan("^ /(|s|) /':='", Color.Whitespace, Color.Structure)
            from expression in getExpression(state, ExpressionFlags.Standard)
            from and in getAnd(state)
            from block in getBlock(state)
            select (comparisand, expression, and, block);

         if (result.ValueOrCast<Unit>(out var tuple, out var asUnit))
         {
            var (comparisand, expression, and, block) = tuple;
            var elseBlock = none<Block>();
            var elseParser = new ElseParser();
            if (elseParser.Scan(state).If(out _, out var anyException))
            {
               elseBlock = elseParser.Block;
            }
            else if (anyException.If(out var exception))
            {
               state.RollBackTransaction();
               return failedMatch<Unit>(exception);
            }

            if (and.If(out var a))
            {
               var builder = new ExpressionBuilder(ExpressionFlags.Comparisand);
               builder.Add(a);
               if (builder.ToExpression().IfNot(out expression, out var exception))
               {
                  state.RollBackTransaction();
                  return failedMatch<Unit>(exception);
               }
            }

            state.CommitTransaction();
            state.AddStatement(new ConditionalAssign(comparisand, expression, block, elseBlock));

            return Unit.Matched();
         }
         else
         {
            state.RollBackTransaction();
            return asUnit;
         }
      }
   }
}