namespace Kagami.Library.Parsers.Expressions;

public class InfixParser(ExpressionBuilder builder) : MultiParser
{
   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new UserOperatorParser(builder);
         yield return new ZipOperatorParser(builder);
         yield return new ZipLambdaParser(builder);
         yield return new RangeOperatorParser(builder);

         if (!builder.Flags[ExpressionFlags.OmitColon] && !builder.Flags[ExpressionFlags.OmitNameValue])
         {
            yield return new KeyValueOperatorParser(builder);
         }

         if (!builder.Flags[ExpressionFlags.OmitBind])
         {
            yield return new BindParser(builder);
         }

         yield return new OperatorsParser(builder);

         if (!builder.Flags[ExpressionFlags.OmitIn])
         {
            yield return new InParser(builder);
         }

         yield return new TwoKeywordOperatorsParser(builder);
         yield return new KeywordOperatorsParser(builder);
      }
   }
}