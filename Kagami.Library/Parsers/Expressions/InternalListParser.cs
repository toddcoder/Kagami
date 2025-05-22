using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class InternalListParser : SymbolParser
{
   public InternalListParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => "^ /(/s*) /'in' /b";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      return getInternalList(state).Map(list =>
      {
         list.ExpandInTuple = false;
         builder.Add(new InternalListSymbol(list));

         return unit;
      });
   }
}