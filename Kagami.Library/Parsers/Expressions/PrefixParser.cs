namespace Kagami.Library.Parsers.Expressions;

public class PrefixParser(ExpressionBuilder builder) : MultiParser
{
   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new UserOperatorParser(builder);

         if (builder.Flags[ExpressionFlags.Comparisand])
         {
            yield return new ComparisandBooleanParser(builder);
         }

         yield return new PrefixIncrementParser(builder);
         yield return new NegateParser(builder);
         yield return new ImageParser(builder);
         yield return new IteratorParser(builder);

         if (!builder.Flags[ExpressionFlags.Comparisand])
         {
            yield return new NotParser(builder);
         }

         yield return new RangePrefixParser(builder);
         yield return new BNotParser(builder);
         yield return new TakeOperatorParser(builder);
         yield return new ImplicitOperatorParser(builder);
         yield return new BindingParser(builder);
      }
   }
}