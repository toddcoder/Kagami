using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class ImplicitParameterParser : SymbolParser
{
   public ImplicitParameterParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(?<![!#\?\^])(\^)(\w)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var letter = tokens[3].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Identifier, Color.Identifier);

      builder.Add(new DollarFieldSymbol($"__${letter}"));
      return unit;
   }
}