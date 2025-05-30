﻿namespace Kagami.Library.Parsers.Statements;

public class ClassItemsParser : MultiParser
{
   protected ClassBuilder builder;

   public ClassItemsParser(ClassBuilder builder) => this.builder = builder;

   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new ConstructorParser(builder);
         yield return new NamedStaticParser();
         yield return new StaticParser(builder);
      }
   }
}