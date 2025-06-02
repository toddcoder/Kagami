using System.Text.RegularExpressions;
using Kagami.Library.Invokables;
using Core.Monads;

namespace Kagami.Library.Parsers.Expressions;

public partial class ZeroParameterLambdaParser : LambdaParser
{
   //public override string Pattern => "^ (> (/s*) ('->' | '=>' [/r/n]+))";

   [GeneratedRegex(@"^(?=(?:[ \t]*)(?:->|=>[\r\n]+))")]
   public override partial Regex Regex();

   public ZeroParameterLambdaParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override Optional<Parameters> ParseParameters(ParseState state, Token[] tokens) => new Parameters(0);
}