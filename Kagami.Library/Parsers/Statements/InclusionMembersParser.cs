using Kagami.Library.Inclusions;

namespace Kagami.Library.Parsers.Statements;

public class InclusionMembersParser(Inclusion inclusion) : MultiParser
{
   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new RequiredOrOptionalFunctionParser(inclusion);
      }
   }
}