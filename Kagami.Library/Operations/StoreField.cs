using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class StoreField(string fieldName, bool mutable, bool visible, bool overriden, Maybe<TypeConstraint> _typeConstraint) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      var _result =
         from newField in machine.CurrentFrame.Fields.New(fieldName, FieldType.Assignment, _typeConstraint, mutable, visible, overriden)
         from assigned in machine.Assign(fieldName, value, false)
         select unit;
      if (_result)
      {
         return nil;
      }
      else
      {
         return _result.Exception;
      }
   }

   public override string ToString() => $"store.field({fieldName}, {mutable.ToString().ToLower()}, {visible.ToString().ToLower()}, {overriden.ToString().ToLower()})";
}