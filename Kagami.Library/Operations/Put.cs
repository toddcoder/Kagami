using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class Put : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      switch (value)
      {
         case Sequence sequence:
         {
            var list = sequence.List;
            if (list.Count == 2)
            {
               machine.Context.Put(stringOf(list[0]), stringOf(list[1]));
               return list[0].Just();
            }

            break;
         }
         case Junction junction:
         {
            foreach (var item in junction.Items)
            {
               machine.Context.Put(stringOf(item));
            }

            break;
         }
         default:
            machine.Context.Put(stringOf(value));
            break;
      }

      return value.Just();
   }

   public override string ToString() => "put";
}