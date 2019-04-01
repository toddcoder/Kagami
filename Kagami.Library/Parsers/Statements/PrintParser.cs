using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;

namespace Kagami.Library.Parsers.Statements
{
   public class PrintParser : EndingInExpressionParser
   {
      bool newLine;

      public override string Pattern => "^ /'print' /'ln'? /(|s+|)";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         newLine = tokens[2].Text == "ln";
         state.Colorize(tokens, Color.Keyword, Color.Keyword, Color.Whitespace);

         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         state.AddStatement(new Print(newLine, expression));
         return Unit.Matched();
      }
   }
}