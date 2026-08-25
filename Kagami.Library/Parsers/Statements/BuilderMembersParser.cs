namespace Kagami.Library.Parsers.Statements;

public class BuilderMembersParser(BuilderState builderState, bool first) : StatementsParser
{
   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new BuilderAssignParser(builderState, first);
         yield return new BuilderReturnParser(builderState);
      }
   }
}