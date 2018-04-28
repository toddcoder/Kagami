using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Expressions
{
   public class NegateParser : SymbolParser
   {
      public NegateParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'-' -(> '>')";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator);
         builder.Add(new NegateSymbol());

         return Unit.Matched();
      }
   }
}