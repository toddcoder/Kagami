using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Runtime;

public class GuardedField(string fieldName, Lambda predicate, Maybe<IObject> _failure) : Field
{
   public override IObject Value
   {
      get => base.Value;
      set
      {
         if (value is Unassigned || predicate.Invoke(value).IsTrue)
         {
            base.Value = value;
         }
         else if (_failure is (true, var failure))
         {
            throw fail(sendMessage(failure, "message".get()).AsString);
         }
         else
         {
            throw guardedFieldPredicateFailed(fieldName);
         }
      }
   }
}