using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;

namespace Kagami.Library.Parsers.Expressions
{
   public class ToEndParser : SymbolParser
   {
      public override string Pattern => "^ /'..' (>[')]'])";

      public ToEndParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Operator);
         builder.Add(new PushObjectSymbol(End.Value));

         return Unit.Matched();
      }
   }
}