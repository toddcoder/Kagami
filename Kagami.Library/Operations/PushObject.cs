using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class PushObject : Operation
{
   protected IObject obj;

   public PushObject(IObject obj) => this.obj = obj;

   public override Optional<IObject> Execute(Machine machine)
   {
      if (obj is Lambda lambda)
      {
         lambda.Capture();
      }

      return obj.Just();
   }

   public override string ToString() => $"push.object({obj.Image})";
}