using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class Concatenate : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      switch (x, y)
      {
         case (KString xs, KString ys):
            return (KString)(xs.Value + ys.Value);
         case (KArray xa, KArray xb):
            return xa.Concatenate(xb).Just();
         case (KTuple xt, KTuple yt):
            return xt.Concatenate(yt);
         default:
         {
            if (x is KString || y is KString)
            {
               return (KString)(x.AsString + y.AsString);
            }
            else
            {
               var _class = Module.Global.Value.Class(x.ClassName);
               if (_class is (true, var @class))
               {
                  return @class.SendMessage(x, "~(_)", new Arguments(y)).Just();
               }
               else
               {
                  return classNotFound(x.ClassName);
               }
            }
         }
      }
   }

   public override string ToString() => "concatenate";
}