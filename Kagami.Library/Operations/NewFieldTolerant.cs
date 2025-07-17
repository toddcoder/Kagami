using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class NewFieldTolerant(string name, bool mutable, bool visible, Maybe<TypeConstraint> _typeConstraint) : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var _field = machine.CurrentFrame.Fields.Find(name, true);
      if (_field)
      {
         return nil;
      }

      var _result = machine.CurrentFrame.Fields.New(name, FieldType.Assignment, _typeConstraint, mutable, visible);
      if (_result)
      {
         return nil;
      }
      else
      {
         return _result.Exception;
      }
   }

   public override string ToString() => "new.field.tolerant";
}