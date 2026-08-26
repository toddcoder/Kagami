namespace Kagami.Library.Parsers.Statements;

public class BuilderMembersParser(BuilderState builderState) : StatementsParser
{
   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new BuilderAssignParser(builderState);
         yield return new BuilderReturnParser(builderState);
         yield return new BuilderDoParser(builderState);
      }
   }
}