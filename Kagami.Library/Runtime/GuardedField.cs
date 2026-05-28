using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Runtime;

public class GuardedField(string fieldName, Lambda predicate) : Field
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
         else
         {
            throw guardedFieldPredicateFailed(fieldName);
         }
      }
   }
}