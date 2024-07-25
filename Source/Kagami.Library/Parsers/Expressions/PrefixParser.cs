using System.Collections.Generic;

namespace Kagami.Library.Parsers.Expressions
{
   public class PrefixParser : MultiParser
   {
      protected ExpressionBuilder builder;

      public PrefixParser(ExpressionBuilder builder) => this.builder = builder;

      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new NegateParser(builder);
            yield return new ImageParser(builder);
            yield return new IteratorParser(builder);
            yield return new NotParser(builder);
            yield return new RangePrefixParser(builder);
            yield return new BNotParser(builder);
            yield return new TakeOperatorParser(builder);
         }
      }
   }
}