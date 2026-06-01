using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class StoreGuardedField(string fieldName, bool mutable, bool visible, Maybe<TypeConstraint> _typeConstraint, bool popFailure)
   : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      Maybe<IObject> _failure = nil;
      if (popFailure)
      {
         _failure = machine.Pop().Maybe();
      }

      if (x is Lambda predicate)
      {
         var _result =
            from newField in machine.CurrentFrame.Fields.NewGuarded(fieldName, FieldType.Assignment, _typeConstraint, predicate, _failure, mutable,
               visible)
            from assigned in machine.Assign(fieldName, y, false)
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
      else
      {
         return expectedType("Lambda");
      }
   }

   public override string ToString() => "store.guarded.field";
}