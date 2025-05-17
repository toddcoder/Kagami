using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class Compare : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      if (x is IObjectCompare cx)
      {
         return Int.IntObject(cx.Compare(y)).Just();
      }
      else
      {
         return sendMessage(x, "<>", y).Just();
      }
   }

   public override string ToString() => "compare";
}