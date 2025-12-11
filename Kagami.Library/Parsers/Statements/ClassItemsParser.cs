namespace Kagami.Library.Parsers.Statements;

public class ClassItemsParser : MultiParser
{
   protected ClassBuilder builder;
   protected bool includeMixinParser;

   public ClassItemsParser(ClassBuilder builder, bool includeMixinParser)
   {
      this.builder = builder;
      this.includeMixinParser = includeMixinParser;
   }

   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new ConstructorParser(builder);

         if (includeMixinParser)
         {
            yield return new MixinParser(builder);
         }

         yield return new StaticParser(builder);
         yield return new DelegateParser(builder);
      }
   }
}