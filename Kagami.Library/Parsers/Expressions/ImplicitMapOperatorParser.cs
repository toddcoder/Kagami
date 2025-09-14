using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class ImplicitMapOperatorParser : SymbolParser
{
   public ImplicitMapOperatorParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\|)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Operator);
      builder.Add(new ImplicitMapSymbol());
      return unit;
   }
}