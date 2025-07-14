using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class PopGetField() : GetField("")
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var _fieldName = machine.Pop();
      if (_fieldName)
      {
         fieldName = _fieldName.Map(f => f.AsString);
         return base.Execute(machine);
      }
      else
      {
         return _fieldName.Exception;
      }
   }

   public override string ToString() => "pop.get.field";
}