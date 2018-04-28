using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes
{
   public class UnassignedClass : BaseClass
   {
      public override string Name => "Unassigned";

      public override bool AssignCompatible(BaseClass otherClass) => true;

      public override void RegisterMessages() { }

      public override bool DynamicRespondsTo(string message) => throw unassigned();

      public override IObject DynamicInvoke(IObject obj, Message message) => throw unassigned();
   }
}