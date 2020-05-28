using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class GuardParser : EndingInExpressionParser
   {
      public override string Pattern => "^ /'guard' /b";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword);
         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         var block = new Block(new Pass());
         var result =
            from keyword in state.Scan("^ /(|s+|) /'else'", Color.Whitespace, Color.Keyword)
            from eBlock in getBlock(state)
            select eBlock;
         if (result.ValueOrCast<Unit>(out var elseBlock, out var asUnit))
         {
            state.AddStatement(new If(expression, block, none<If>(), elseBlock.Some(), "", false, false, true));
            return Unit.Matched();
         }
         else
         {
            return asUnit;
         }
      }
   }
}