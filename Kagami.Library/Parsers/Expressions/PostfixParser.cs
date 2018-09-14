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
            yield return new IndexOptionalParser(builder);
            yield return new IndexParser(builder);
            yield return new SkipTakeOperatorParser2(builder);
	         yield return new WhereParser(builder);

            if (!builder.Flags[ExpressionFlags.OmitSendMessageAssign])
               yield return new SendMessageAssignParser(builder);

            yield return new FoldOperatorParser(builder);
            yield return new SendMessageParser(builder);
         }
      }
   }
}