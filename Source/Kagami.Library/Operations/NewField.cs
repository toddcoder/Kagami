using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class NewField : Operation
   {
      protected string name;
      protected bool mutable;
      protected bool visible;
      protected IMaybe<TypeConstraint> typeConstraint;

      public NewField(string name, bool mutable, bool visible, IMaybe<TypeConstraint> typeConstraint)
      {
         this.name = name;
         this.mutable = mutable;
         this.visible = visible;
         this.typeConstraint = typeConstraint;
      }

      public override IMatched<IObject> Execute(Machine machine)
      {
         return machine.CurrentFrame.Fields.New(name, typeConstraint, mutable, visible).If(out _, out var exception) ? notMatched<IObject>()
            : failedMatch<IObject>(exception);
      }

      public override string ToString() => $"new.field({name}, {mutable.ToString().ToLower()}, {visible.ToString().ToLower()})";
   }
}