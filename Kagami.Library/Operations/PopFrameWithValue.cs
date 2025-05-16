using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class PopFrameWithValue : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var _result =
         from frame in machine.PopFrame()
         from value in frame.Pop()
         select value;
      return _result.Optional();
   }

   public override string ToString() => "pop.frame.with.value";
}