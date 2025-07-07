using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class PopNewField : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      var fieldName = x.AsString;
      var _field = machine.CurrentFrame.Fields.New(fieldName, y);
      if (_field)
      {
         return y.Just();
      }
      else
      {
         return _field.Exception;
      }
   }

   public override string ToString() => "pop.new.field";
}