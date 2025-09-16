using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class RegisterAutoConversion(string fromClass, string toClass) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Lambda lambda)
      {
         var _result = Module.RegisterAutoConversion(fromClass, toClass, lambda);
         return _result.Map(IObject (_) => lambda).Optional();
      }
      else
      {
         return incompatibleClasses(value, "Lambda");
      }
   }

   public override string ToString() => $"register.auto.conversion({fromClass}, {toClass})";
}