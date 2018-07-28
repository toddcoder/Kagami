using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.Operations.OperationFunctions;

namespace Kagami.Library.Operations
{
   public class PushObject : Operation
   {
      IObject obj;

      public PushObject(IObject obj) => this.obj = obj;

      public override IMatched<IObject> Execute(Machine machine)
      {
         if (obj is ICopyFields || obj is IPristineCopy)
            obj = copyFields(obj);
         return obj.Matched();
      }

      public override string ToString() => $"push.object({obj.Image})";
   }
}