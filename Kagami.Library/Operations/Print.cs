using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class Print : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Junction junction)
      {
         foreach (var item in junction.Items)
         {
            machine.Context.Print(stringOf(item));
         }
      }
      else
      {
         machine.Context.Print(stringOf(value));
      }

      return value.Just();
   }

   public override string ToString() => "print";
}