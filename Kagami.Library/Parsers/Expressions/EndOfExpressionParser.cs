using System.Text.RegularExpressions;
using Core.Monads;
//using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class EndOfExpressionParser : SymbolParser
{
   protected ConjunctionParsers conjunctionParsers;

   public EndOfExpressionParser(ExpressionBuilder builder, ConjunctionParsers conjunctionParsers) : base(builder)
   {
      this.conjunctionParsers = conjunctionParsers;
   }

   [GeneratedRegex(@"^(\s*)(\.)(?=\s|$)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);
      conjunctionParsers.IsEndOfExpression = true;

      return unit;
   }
}