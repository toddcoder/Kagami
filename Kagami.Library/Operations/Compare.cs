using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations
{
   public class Compare : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         if (x is IObjectCompare cx)
            return Int.IntObject(cx.Compare(y)).Matched();
         else
            return sendMessage(x, "<>", y).Matched();
      }

      public override string ToString() => "compare";
   }
}