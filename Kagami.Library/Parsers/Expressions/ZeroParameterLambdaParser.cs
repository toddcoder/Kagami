using Kagami.Library.Invokables;
using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Expressions
{
   public class ZeroParameterLambdaParser : LambdaParser
   {
      public override string Pattern => "^ (> (|s|) ('->' | '=>'))";

      public ZeroParameterLambdaParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Parameters> ParseParameters(ParseState state, Token[] tokens) => new Parameters(0).Matched();
   }
}