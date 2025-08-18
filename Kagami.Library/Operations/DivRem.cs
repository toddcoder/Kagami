using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class DivRem : TwoNumericOperation
{
   public override Optional<IObject> Execute(INumeric x, INumeric y)
   {
      try
      {
         switch (x)
         {
            case IMessageNumber messageX:
               return messageX.DivRem(y).Just();
            case Int intX:
            {
               var intY = y.AsInt32();
               var div = new Int(intX.Value / intY);
               var rem = new Int(intX.Value % intY);
               var name1 = new NameValue("div", div);
               var name2 = new NameValue("rem", rem);

               return new KTuple(name1, name2);
            }
            default:
               return incompatibleClasses((IObject)x, "MessageNumber");
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public override string ToString() => "div.rem";
}