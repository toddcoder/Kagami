using Kagami.Library.Inclusions;

namespace Kagami.Library.Parsers.Statements;

public class InclusionMembersParser(Inclusion inclusion) : MultiParser
{
   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new RequiredFieldParser(inclusion);
         yield return new RequiredPropertyParser(inclusion);
         yield return new RequiredFunctionParser(inclusion);
         yield return new InclusionFunctionParser(inclusion);
      }
   }
}