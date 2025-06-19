using System.Text.RegularExpressions;
using Kagami.Library.Invokables;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class MultiParameterLambdaParser : LambdaParser
{
   [GeneratedRegex(@"^([ \t]*)(\()")]
   public override partial Regex Regex();

   public MultiParameterLambdaParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override Optional<Parameters> ParseParameters(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.OpenParenthesis);
      return getParameters(state);
   }
}