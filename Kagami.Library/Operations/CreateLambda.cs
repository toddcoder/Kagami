using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class CreateLambda : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      try
      {
         var expression = value.AsString;
         var compiler = new Compiler($"-> ${expression}", new CompilerConfiguration(), machine.Context);
         var _operations = compiler.Generate().Map(_ => compiler.Operations.Result("No operations available"));
         if (_operations is (true, var operations))
         {
            var targetAddress = machine.Operations.Count;
            machine.Operations.AppendStop();
            machine.Operations.Append(operations);

            var createdInvokable = new CreatedInvokable(targetAddress, expression);
            var lambda = new Lambda(createdInvokable, false);
            return ((IObject)lambda).Just();
         }
         else
         {
            return _operations.Exception;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public override string ToString() => "create.lambda";
}