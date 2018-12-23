using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;

namespace Kagami.Library.Operations
{
   public class NewObject : Operation
   {
      string className;
      Parameters parameters;

      public NewObject(string className, Parameters parameters)
      {
         this.className = className;
         this.parameters = parameters;
      }

      public override IMatched<IObject> Execute(Machine machine)
      {
         return new UserObject(className, machine.CurrentFrame.Fields, parameters).Matched<IObject>();
      }

      public override string ToString() => $"new.object({className}, {parameters})";
   }
}