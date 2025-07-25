namespace Kagami.Library.Classes;

public class UndefinedClass : BaseClass
{
   public override string Name => "Undefined";

   public override bool AssignCompatible(BaseClass otherClass) => true;
}