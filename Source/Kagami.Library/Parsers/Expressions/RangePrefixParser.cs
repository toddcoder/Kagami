using Kagami.Library.Nodes.Symbols;
using Core.Monads;

namespace Kagami.Library.Parsers.Expressions
{
   public class RangePrefixParser : SymbolParser
   {
      public RangePrefixParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'+' -(> /s+)";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator);
         builder.Add(new SendPrefixMessage("range"));

         return Unit.Matched();
      }
   }
}