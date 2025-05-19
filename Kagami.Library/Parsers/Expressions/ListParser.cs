using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ListParser : SymbolParser
{
   public ListParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => $"^ /(/s*) /'{REGEX_LIST_LEFT}' /(/s*)";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Collection, Color.Whitespace);

      var _expression = getExpression(state, $"^ /(/s*) /'{REGEX_LIST_RIGHT}'", builder.Flags & ~ExpressionFlags.OmitComma, Color.Whitespace, Color.Collection);
      if (_expression is (true, var expression))
      {
         builder.Add(new ListSymbol(expression));
         return unit;
      }
      else
      {
         return nil;
      }
   }
}