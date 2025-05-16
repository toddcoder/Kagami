using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class NewField : Operation
{
   protected string name;
   protected bool mutable;
   protected bool visible;
   protected Maybe<TypeConstraint> _typeConstraint;

   public NewField(string name, bool mutable, bool visible, Maybe<TypeConstraint> _typeConstraint)
   {
      this.name = name;
      this.mutable = mutable;
      this.visible = visible;
      this._typeConstraint = _typeConstraint;
   }

   public override Optional<IObject> Execute(Machine machine)
   {
      return machine.CurrentFrame.Fields.New(name, _typeConstraint, mutable, visible).Map(f => (IObject)f).Optional();
   }

   public override string ToString() => $"new.field({name}, {mutable.ToString().ToLower()}, {visible.ToString().ToLower()})";
}