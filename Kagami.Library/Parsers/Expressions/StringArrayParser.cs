using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class StringArrayParser : SymbolParser
{
   [GeneratedRegex("""^(\s*)(a")([^"]*)(")""")]
   public override partial Regex Regex();

   public StringArrayParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var source = tokens[3].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Collection, Color.Collection, Color.Collection);

      builder.Add(new StringArraySymbol(source));

      return unit;
   }
}