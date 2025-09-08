using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class Defer : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Lambda lambda)
      {
         var _functionFrame = machine.FunctionFrame();
         if (_functionFrame is (true, var functionFrame))
         {
            functionFrame.Defer(lambda);
            return nil;
         }
         else
         {
            return fail("No function frame");
         }
      }
      else
      {
         return incompatibleClasses(value, "Lambda");
      }
   }

   public override string ToString() => "defer";
}