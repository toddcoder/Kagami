using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Expressions
{
   public class IteratorParser : SymbolParser
   {
      public IteratorParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /['^!'] -(> /s)";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var lazy = tokens[2].Text == "^";
         state.Colorize(tokens, Color.Whitespace, Color.Operator);
         builder.Add(new IteratorSymbol(lazy));

         return Unit.Matched();
      }
   }
}