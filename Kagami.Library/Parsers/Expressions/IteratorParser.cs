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

   [GeneratedRegex(@"^(\s*)(!{1,2}|\?)(?![\s\^\(>])")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var lazy = tokens[2].Text == "!!";
      var indexed = tokens[2].Text == "?";
      state.Colorize(tokens, Color.Whitespace, Color.Operator);

      builder.Add(new IteratorSymbol(lazy, indexed));
      return unit;
   }
}