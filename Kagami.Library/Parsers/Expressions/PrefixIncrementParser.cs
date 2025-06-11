using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class PrefixIncrementParser : SymbolParser
{
   public PrefixIncrementParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\+\+|--)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var increment = tokens[2].Text == "++";
      state.Colorize(tokens, Color.Whitespace, Color.Operator);

      builder.Add(new PreIncrementSymbol(increment));

      return unit;
   }
}