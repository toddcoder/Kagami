using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Nodes.NodeFunctions;
using Yield = Kagami.Library.Nodes.Statements.Yield;

namespace Kagami.Library.Parsers.Statements
{
   public class YieldParser : EndingInExpressionParser
   {
      bool all;

      public override string Pattern => "^ /'yield' (/(|s+|) /'all')? /(|s+|)";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         all = tokens[3].Text == "all";
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Keyword, Color.Whitespace);
         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         if (all)
         {
	         var placeholderName = newLabel("yieldIndex");
            var block = new Block(new Yield(new Expression(new FieldSymbol(placeholderName))));
            var for2 = new For2(new PlaceholderSymbol("-" + placeholderName), expression, block);
            state.AddStatement(for2);
         }
         else
         {
	         state.AddStatement(new Yield(expression));
         }

         state.SetYieldFlag();
         return Unit.Matched();
      }
   }
}