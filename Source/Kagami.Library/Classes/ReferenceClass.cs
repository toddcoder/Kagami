namespace Kagami.Library.Classes
{
   public class ReferenceClass : BaseClass
   {
      public override string Name => "Reference";

      public override bool AssignCompatible(BaseClass otherClass) => true;
   }
}