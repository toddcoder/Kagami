using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class NewDefinition : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Lambda lambda)
      {
         return new Definition(lambda);
      }
      else
      {
         return expectedType("Definition");
      }
   }
}