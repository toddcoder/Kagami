using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class NewNameValue : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      if (x is String s)
      {
         return new NameValue(s.Value, y);
      }
      else
      {
         return incompatibleClasses(x, "String");
      }
   }

   public override string ToString() => "new.name.value";
}