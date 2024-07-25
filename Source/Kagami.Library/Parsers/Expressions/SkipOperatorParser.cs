using Core.Monads;
using Kagami.Library.Nodes.Symbols;

namespace Kagami.Library.Parsers.Expressions
{
   public class SkipOperatorParser : SymbolParser
   {
      public SkipOperatorParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /';*'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator);
         builder.Add(new SkipOperatorSymbol());

         return Unit.Matched();
      }
   }
}