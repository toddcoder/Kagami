using Kagami.Library.Nodes.Symbols;

namespace Kagami.Library.Parsers.Expressions;

public class PostfixParser(ExpressionBuilder builder) : MultiParser
{
   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new UserOperatorParser(builder, Arity.Postfix);
         //yield return new SomeSuccessParser(builder);
         yield return new PostfixIncrementParser(builder);
         yield return new InfiniteRangeParser(builder);
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
         //yield return new AsOperatorParser(builder);

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