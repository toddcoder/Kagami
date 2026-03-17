using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class ForwardReduction(string operationSource) : OneOperandOperation
{
   protected Optional<IObject> execute(IIterator iterator, Machine machine)
   {
      var expression = iterator.Join($" {operationSource} ").Value;
      var _operation = machine.Operations.CreateOperationsFromExpression(expression, machine.Address);
      if (_operation is (true, var operation))
      {
         return operation.Execute(machine);
      }
      else
      {
         return _operation.Exception;
      }
   }

   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      switch (value)
      {
         case ICollection collection:
         {
            var iterator = collection.GetIterator(false);
            return execute(iterator, machine);
         }
         case IIterator iterator:
         {
            return execute(iterator, machine);
         }
         default:
            return expectedType("Collection or Iterator");
      }
   }

   public override string ToString() => "forward.reduction";
}