namespace Kagami.Library.Classes
{
   public class NumberClass : BaseClass
   {
      public override string Name => "Number";

      public override bool MatchCompatible(BaseClass otherClass) => otherClass.IsNumeric;

      public override bool AssignCompatible(BaseClass otherClass) => otherClass.IsNumeric;
   }
}