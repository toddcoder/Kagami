using System.Collections.Generic;
using Kagami.Library.Classes;

namespace Kagami.Library.Parsers.Statements
{
   public class TraitItemsParser : MultiParser
   {
      TraitClass traitClass;

      public TraitItemsParser(TraitClass traitClass) => this.traitClass = traitClass;

      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new FunctionParser { TraitName = traitClass.Name };
            yield return new SignatureParser(traitClass);
         }
      }
   }
}