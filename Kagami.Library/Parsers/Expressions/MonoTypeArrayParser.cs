using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class MonoTypeArrayParser : SymbolParser
{
   public MonoTypeArrayParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(<)([^>]+)(>)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var source = tokens[3].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Collection, Color.Collection, Color.Collection);

      builder.Add(new MonoTypeArraySymbol(source));
      return unit;
   }
}