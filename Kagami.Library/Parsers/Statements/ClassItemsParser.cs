using System.Collections.Generic;

namespace Kagami.Library.Parsers.Statements
{
   public class ClassItemsParser : MultiParser
   {
      ClassBuilder builder;

      public ClassItemsParser(ClassBuilder builder) => this.builder = builder;

      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new ConstructorParser(builder);
            yield return new StaticParser(builder);
         }
      }
   }
}