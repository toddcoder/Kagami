using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using Core.Monads.Lazy;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class NewContainer : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      LazyResult<IObject> _x = nil;
      var _y = machine.Pop();
      if (_y is (true, var y))
      {
         if (machine.IsEmpty)
         {
            return new KTuple(y);
         }
         else if (_x.ValueOf(machine.Pop()) is (true, var x))
         {
            if (x is Container container)
            {
               container.Add(y);
               return container;
            }
            else
            {
               return new Container(x, y);
            }
         }
         else
         {
            return _x.Exception;
         }
      }
      else
      {
         return _y.Exception;
      }
   }

   public override string ToString() => "new.container";
}