using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class OneTuple : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         if (value is Tuple)
            return value.Matched();
         else
            return new Tuple(value).Matched<IObject>();
      }

      public override string ToString() => "one.tuple";
   }
}