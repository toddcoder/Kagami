using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class NewObject : Operation
{
   protected string className;
   protected Parameters parameters;

   public NewObject(string className, Parameters parameters)
   {
      this.className = className;
      this.parameters = parameters;
   }

   public override Optional<IObject> Execute(Machine machine)
   {
      return new UserObject(className, machine.CurrentFrame.Fields, parameters);
   }

   public override string ToString() => $"new.object({className}, {parameters})";
}