using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Guards;

public readonly struct Guard(IInvokable invokable, Maybe<IInvokable> _failure)
{
   public Result<Unit> Passes(string parameterName, IObject value)
   {
      var field = new Field { Mutable = false, Type = FieldType.Assignment, Value = value };
      var fields = new Fields();
      fields.New(parameterName, field);
      var _guardValue = Machine.Current.Invoke(invokable, new Arguments(value), fields, false);
      if (_guardValue is (true, var guardValue))
      {
         if (guardValue.IsTrue)
         {
            return unit;
         }
         else if (_failure is (true, var failure))
         {
            var _failureValue = Machine.Current.Invoke(failure, Arguments.Empty, nil)
               .Map(f => f is KString kString ? kString.AsString : sendMessage(f, "message".get()).AsString);
            if (_failureValue is (true, var failureString))
            {
               return fail(failureString);
            }
            else
            {
               return guardedFieldPredicateFailed(parameterName);
            }
         }
         else
         {
            return guardedFieldPredicateFailed(parameterName);
         }
      }
      else if (_guardValue.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return unit;
      }
   }
}