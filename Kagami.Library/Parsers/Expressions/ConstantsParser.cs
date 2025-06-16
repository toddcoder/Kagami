namespace Kagami.Library.Parsers.Expressions;

public class ConstantsParser : MultiParser
{
   protected ExpressionBuilder builder;

   public ConstantsParser(ExpressionBuilder builder) => this.builder = builder;

   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new AnyParser(builder);
         yield return new FloatParser(builder);
         yield return new ByteParser(builder);
         yield return new IntParser(builder);
         yield return new BooleanParser(builder);
         yield return new StringParser(builder);
         yield return new CharParser(builder);
      }
   }
}