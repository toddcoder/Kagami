using System.Collections.Generic;

namespace Kagami.Library.Parsers.Expressions
{
   public class PrefixOperatorsParser : MultiParser
   {
      ExpressionBuilder builder;

      public PrefixOperatorsParser(ExpressionBuilder builder) => this.builder = builder;

      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new NegateParser(builder);
            yield return new ImageParser(builder);
         }
      }
   }
}