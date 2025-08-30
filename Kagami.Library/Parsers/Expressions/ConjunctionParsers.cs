namespace Kagami.Library.Parsers.Expressions;

public class ConjunctionParsers(ExpressionBuilder builder) : MultiParser
{
   public override IEnumerable<Parser> Parsers
   {
      get
      {
         IsEndOfExpression = false;

         yield return new IsParser(builder);
         yield return new AsOperatorParser(builder);
         yield return new WhereParser(builder);
         yield return new MatchExpressionParser(builder);

         if (!builder.Flags[ExpressionFlags.OmitComprehension])
         {
            yield return new ComprehensionParser(builder);
         }

         if (!builder.Flags[ExpressionFlags.OmitAnd])
         {
            yield return new AndParser(builder);
         }

         yield return new OrParser(builder);

         if (!builder.Flags[ExpressionFlags.Comparisand])
         {
            yield return new InlineIfParser(builder);
         }
         yield return new ImplicitCollectionExpressionParser(builder);
         yield return new DefaultToParser(builder);
      }
   }

   public bool IsEndOfExpression { get; set; }
}