using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Booleans;
using Core.Monads;
using static Core.Monads.MonadFunctions;

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
      var _field = machine.Assign(name, value, false);
      if (_field)
      {
         return nil;
      }
      else
      {
         return _field.Exception;
      }
   }

   public override string ToString() => $"assign.field({name}{overriding.Extend(", override")})";
}