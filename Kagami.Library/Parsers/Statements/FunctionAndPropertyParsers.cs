namespace Kagami.Library.Parsers.Statements;

public class FunctionAndPropertyParsers(string className, string selfAlias) : MultiParser
{
   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new FunctionParser { ClassName = className, SelfAlias = selfAlias };
         yield return new PropertyParser { ClassName = className, SelfAlias = selfAlias };
         yield return new AutoPropertyParser { ClassName = className };
      }
   }
}