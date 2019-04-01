using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class PushObject : Operation
   {
      IObject obj;

      public PushObject(IObject obj) => this.obj = obj;

      public override IMatched<IObject> Execute(Machine machine)
      {
         if (obj is Lambda lambda)
            lambda.Capture();
         return obj.Matched();
      }

      public override string ToString() => $"push.object({obj.Image})";
   }
}