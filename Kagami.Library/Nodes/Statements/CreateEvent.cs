using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Nodes.Statements;

public class CreateEvent(string eventName) : Statement, IFieldStatement
{
   protected TypeConstraint typeConstraint = Objects.TypeConstraint.SingleType(classOf("Event"));

   public override void Generate(OperationsBuilder builder)
   {
      var kEvent = new KEvent();
      builder.PushObject(kEvent);
      builder.StoreField(eventName, false, true, typeConstraint);
   }

   public string EventName => eventName;

   public override string ToString() => $"event {eventName}";
   public string Name => eventName;

   public bool Mutable => false;

   public Maybe<TypeConstraint> TypeConstraint => typeConstraint;
}