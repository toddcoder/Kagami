using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class IteratorParser : SymbolParser
{
   public IteratorParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(it|lazy it|indexed it|lit|iit|rng|range)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var lazy = tokens[2].Text is "lazy it" or "lit";
      var indexed = tokens[2].Text is "indexed it" or "iit";
      var range = tokens[2].Text is "rng" or "range";
      state.Colorize(tokens, Color.Whitespace, Color.Operator);

      builder.Add(new IteratorSymbol(lazy, indexed, range));
      return unit;
   }
}