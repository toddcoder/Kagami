namespace Kagami.Library.Parsers.Statements;

public class ProtocolMembersParser(ProtocolBuilder builder) : MultiParser
{
   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new ProtocolFieldParser(builder);
         yield return new ProtocolGetterParser(builder);
         yield return new ProtocolSetterParser(builder);
         yield return new ProtocolFunctionParser(builder);
      }
   }
}