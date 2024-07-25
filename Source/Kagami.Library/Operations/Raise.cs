using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations
{
   public class Raise : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         if (x is INumeric n1 && y is INumeric n2)
         {
	         return n1.Raise(n2).Matched();
         }
         else
         {
	         return sendMessage(x, "^", y).Matched();
         }
      }

      public override string ToString() => "raise";
   }
}