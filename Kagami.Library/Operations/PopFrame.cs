using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class PopFrame : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var _frame = machine.PopFrame();
      if (_frame)
      {
         return nil;
      }
      else
      {
         return _frame.Exception;
      }
   }

   public override string ToString() => "pop.frame";
}