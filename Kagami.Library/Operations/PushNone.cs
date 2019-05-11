using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class PushNone : Operation
   {
      public override IMatched<IObject> Execute(Machine machine) => None.NoneValue.Matched();

      public override string ToString() => "push.none";
   }
}