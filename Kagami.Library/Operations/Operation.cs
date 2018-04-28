using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public abstract class Operation
   {
      public abstract IMatched<IObject> Execute(Machine machine);

      public virtual bool Increment => true;
   }
}