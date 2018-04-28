using System.Collections.Generic;

namespace Kagami.Library.Parsers.Expressions
{
   public class PostfixParser : MultiParser
   {
      protected ExpressionBuilder builder;

      public PostfixParser(ExpressionBuilder builder) => this.builder = builder;

      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new PostfixOperatorsParser(builder);
            yield return new PostfixInvokeParser(builder);
            yield return new IndexParser(builder);

            if (!builder.Flags[ExpressionFlags.OmitSendMessageAssign])
               yield return new SendMessageAssignParser(builder);

            yield return new SendMessageParser(builder);
         }
      }
   }
}