using System.Collections.Generic;

namespace Kagami.Library.Parsers.Expressions
{
   public class PrefixParser : MultiParser
   {
      ExpressionBuilder builder;

      public PrefixParser(ExpressionBuilder builder) => this.builder = builder;

      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new NegateParser(builder);
            yield return new ImageParser(builder);
            yield return new IteratorParser(builder);
            yield return new NotParser(builder);
            yield return new BNotParser(builder);
         }
      }
   }
}