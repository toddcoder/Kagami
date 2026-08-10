using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

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
         case (KChar xc, KChar yc):
            return (KString)(xc.AsString + yc.AsString);
         case (INumeric xn, KString ys):
            return (KString)(((IObject)xn).AsString + ys.Value);
         case (KString xs, INumeric yn):
            return (KString)(xs.Value + ((IObject)yn).AsString);
         case (INumeric xn, INumeric yn):
            return (KString)(((IObject)xn).AsString + ((IObject)yn).AsString);
         default:
         {
            var _class = Module.Global.Value.Class(x.ClassName);
            if (_class is (true, var @class))
            {
               if (@class.RespondsTo("~(_)"))
               {
                  return @class.SendMessage(x, "~(_)", new Arguments(y)).Just();
               }
               else
               {
                  return KString.StringObject(stringOf(x) + stringOf(y)).Just();
               }
            }
            else
            {
               return classNotFound(x.ClassName);
            }
         }
      }
   }

   public override string ToString() => "concatenate";
}