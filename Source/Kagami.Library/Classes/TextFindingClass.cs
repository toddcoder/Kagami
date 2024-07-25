namespace Kagami.Library.Classes
{
   public class TextFindingClass : BaseClass
   {
      public override string Name => "TextFinding";

      public override bool MatchCompatible(BaseClass otherClass)
      {
         return otherClass is StringClass || otherClass is CharClass || otherClass is RegexClass;
      }

      public override bool AssignCompatible(BaseClass otherClass) => MatchCompatible(otherClass);
   }
}