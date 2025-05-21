namespace Kagami.Library.Parsers.Expressions;

public class PostfixParser : MultiParser
{
   protected ExpressionBuilder builder;

   public PostfixParser(ExpressionBuilder builder) => this.builder = builder;

   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new ToEndParser(builder);
         yield return new PostfixOperatorsParser(builder);
         yield return new PostfixInvokeParser(builder);
         yield return new IndexOptionalParser(builder);
         yield return new IndexerParser(builder);
         yield return new ReductionParser(builder);
         yield return new SliceAssignParser(builder);
         yield return new SliceParser(builder);
         yield return new WhereParser(builder);
         yield return new SkipOperatorParser(builder);

         if (!builder.Flags[ExpressionFlags.OmitSendMessageAssign])
         {
            yield return new SendMessageAssignParser(builder);
            yield return new SendBindingMessageParser(builder);
         }

         yield return new FoldOperatorParser(builder);
         yield return new SendMessageParser(builder);
      }
   }
}