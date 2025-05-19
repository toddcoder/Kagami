using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using Core.Monads.Lazy;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class TryEnd : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      LazyResult<IObject> _value = nil;
      if (machine.IsEmpty)
      {
         Optional<IObject> _result = new Objects.Success(KUnit.Value);
         machine.PopFramesUntil(f => f.FrameType == FrameType.Try);
         return _result;
      }
      else if (_value.ValueOf(machine.Pop()) is (true, var value))
      {
         IObject result = value switch
         {
            Objects.Success success => success,
            Objects.Failure failure => failure,
            _ => new Objects.Success(value)
         };

         return result.Just();
      }
      else
      {
         return _value.Exception;
      }
   }

   public override string ToString() => "try.end";
}