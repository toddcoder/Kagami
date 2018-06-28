using System.Collections.Generic;

namespace Kagami.Library.Parsers.Expressions
{
   public class AnyLambdaParser : MultiParser
   {
      ExpressionBuilder builder;

      public AnyLambdaParser(ExpressionBuilder builder) => this.builder = builder;

      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new ZeroParameterLambdaParser(builder);
            yield return new OneParameterLambdaParser(builder);
            yield return new MatchLambdaParser(builder);
            yield return new MultiParameterLambdaParser(builder);
            yield return new PartialLambdaParser(builder);
         }
      }
   }
}