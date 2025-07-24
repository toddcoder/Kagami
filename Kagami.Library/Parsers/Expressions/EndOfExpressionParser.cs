using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class EndOfExpressionParser : SymbolParser
{
   protected ValuesParser valuesParser;

   public EndOfExpressionParser(ExpressionBuilder builder, ValuesParser valuesParser) : base(builder)
   {
      this.valuesParser = valuesParser;
   }

   [GeneratedRegex(@"^(\s*)(\.)(?![\w\d\.])")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);
      valuesParser.IsEndOfExpression = true;

      return unit;
   }
}