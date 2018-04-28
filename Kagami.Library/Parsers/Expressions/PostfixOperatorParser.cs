using System.Collections.Generic;

namespace Kagami.Library.Parsers.Expressions
{
   public class PostfixOperatorParser : MultiParser
   {
      protected ExpressionBuilder builder;
	   bool isComparisand;

      public PostfixOperatorParser(ExpressionBuilder builder, bool isComparisand)
	   {
		   this.builder = builder;
		   this.isComparisand = isComparisand;
	   }

	   public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new SendMessageParser(builder, isComparisand);
         }
      }
   }
}