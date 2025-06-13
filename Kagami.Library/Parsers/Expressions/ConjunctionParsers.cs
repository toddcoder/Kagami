namespace Kagami.Library.Parsers.Expressions;

public class ConjunctionParsers(ExpressionBuilder builder) : MultiParser
{
   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new IsParser(builder);
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
         yield return new InlineIfParser(builder);
         yield return new ImplicitCollectionExpressionParser(builder);
      }
   }
}