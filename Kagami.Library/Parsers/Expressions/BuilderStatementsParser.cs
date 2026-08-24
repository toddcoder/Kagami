using Kagami.Library.Parsers.Statements;

namespace Kagami.Library.Parsers.Expressions;

public class BuilderStatementsParser : StatementsParser
{
   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new AssignToFieldParser();
         yield return new ReturnParser();
      }
   }
}