using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Booleans;
using Core.Monads;
using Core.Monads.Lazy;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class AssignField : OneOperandOperation
{
   protected string name;
   protected bool overriding;

   public AssignField(string name, bool overriding)
   {
      this.name = name;
      this.overriding = overriding;
   }

   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      LazyOptional<Field> _self = nil;
      var _field = machine.Assign(name, value, false, overriding);
      if (_field is (true, var field))
      {
         return field.Value.Just();
      }
      else if (_self.ValueOf(machine.Find("self", true)) is (true, var self))
      {
         sendMessage(self.Value, name.set(), [value]);
         return value.Just();
      }
      else
      {
         return _field.Exception;
      }
   }

   public override string ToString() => $"assign.field({name}{overriding.Extend(", override")})";
}