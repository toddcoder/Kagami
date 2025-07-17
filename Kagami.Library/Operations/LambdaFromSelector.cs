using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class LambdaFromSelector : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Selector selector)
      {
         var _field = machine.CurrentFrame.Fields.Find(selector);
         if (_field is (true, var field))
         {
            var fieldValue = field.Value;
            return fieldValue switch
            {
               Lambda lambda => lambda,
               PackageFunction packageFunction => packageFunction.ToLambda(),
               _ => incompatibleClasses(fieldValue, "Lambda")
            };
         }
         else
         {
            return fail($"Selector {selector} doesn't retrieve a value");
         }
      }
      else
      {
         return incompatibleClasses(value, "Selector");
      }
   }

   public override string ToString() => "lambda.from.selector";
}