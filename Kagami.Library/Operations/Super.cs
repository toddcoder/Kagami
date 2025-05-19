using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class Super : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var _selfField = machine.Find("self", true);
      if (_selfField is (true, var selfField))
      {
         var self = (UserObject)selfField.Value;
         var selfClass = (UserClass)classOf(self);
         var parentClassName = selfClass.ParentClassName;
         if (parentClassName.IsEmpty())
         {
            return fail($"Class {selfClass.Name} has no parent class");
         }
         else
         {
            var superObject = new UserObject(parentClassName, self.Fields, self.Parameters);
            return superObject;
         }
      }
      else if (_selfField.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return fail("self not defined");
      }
   }

   public override string ToString() => "super";
}