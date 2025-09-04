using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class IsTrueParser : SymbolParser
{
   public IsTrueParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(so|but)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var isTrue = tokens[2].Text == "so";
      state.Colorize(tokens, Color.Whitespace, Color.Operator);
      builder.Add(new IsTrueSymbol(isTrue));

      return unit;
   }
}