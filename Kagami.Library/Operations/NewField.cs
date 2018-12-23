using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class NewField : Operation
   {
      string name;
      bool mutable;
      bool visible;
	   IMaybe<TypeConstraint> typeConstraint;

      public NewField(string name, bool mutable, bool visible, IMaybe<TypeConstraint> typeConstraint)
      {
         this.name = name;
         this.mutable = mutable;
         this.visible = visible;
	      this.typeConstraint = typeConstraint;
      }

      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.CurrentFrame.Fields.New(name, typeConstraint, mutable, visible).If(out _, out var exception))
            return notMatched<IObject>();
         else
            return failedMatch<IObject>(exception);
      }

      public override string ToString() => $"new.field({name}, {mutable.ToString().ToLower()}, {visible.ToString().ToLower()})";
   }
}