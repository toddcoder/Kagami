using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class Super : Operation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.Find("self", true).If(out var selfField, out var isNotMatched, out var exception))
         {
            var self = (UserObject)selfField.Value;
            var selfClass = (UserClass)classOf(self);
            var parentClassName = selfClass.ParentClassName;
            if (parentClassName.IsEmpty())
               return $"Class {selfClass.Name} has no parent class".FailedMatch<IObject>();
            else
            {
               var superObject = new UserObject(parentClassName, self.Fields, self.Parameters);
               return superObject.Matched<IObject>();
            }
         }
         else if (isNotMatched)
            return "self not defined".FailedMatch<IObject>();
         else
            return failedMatch<IObject>(exception);
      }

      public override string ToString() => "super";
   }
}