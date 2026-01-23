namespace Kagami.Library.Parsers.Statements;

public class FunctionAndPropertyParsers(string className) : MultiParser
{
   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new FunctionParser { ClassName = className };
         yield return new PropertyParser { ClassName = className };
         yield return new AutoPropertyParser { ClassName = className };
      }
   }
}