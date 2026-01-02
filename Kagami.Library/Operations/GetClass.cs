using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class GetClass : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      if (machine.FunctionFrame() is (true, { CurrentClass: (true, var @class) }))
      {
         return @class;
      }
      else
      {
         return fail("No current class in function frame");
      }
   }

   public override string ToString() => "get.class";
}