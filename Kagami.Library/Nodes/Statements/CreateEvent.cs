using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Nodes.Statements;

public class CreateEvent(string eventName) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      var kEvent = new KEvent();
      builder.PushObject(kEvent);
      builder.StoreField(eventName, false, true, TypeConstraint.SingleType(classOf("Event")));
   }

   public string EventName => eventName;

   public override string ToString() => $"event {eventName}";
}