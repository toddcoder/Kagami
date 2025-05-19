using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class IteratorParser : SymbolParser
{
   public IteratorParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(|s|) /('!'1%2 | '?') -(> [/s '^>'])";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var lazy = tokens[2].Text == "!!";
      var indexed = tokens[2].Text == "?";
      state.Colorize(tokens, Color.Whitespace, Color.Operator);

      builder.Add(new IteratorSymbol(lazy, indexed));
      return unit;
   }
}