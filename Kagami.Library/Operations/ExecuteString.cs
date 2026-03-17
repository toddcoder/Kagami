using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class ExecuteString : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      try
      {
         var expression = value.AsString;
         var compiler = new Compiler(expression, new CompilerConfiguration(), machine.Context);
         var _newMachine = compiler.Generate();
         if (_newMachine is (true, var newMachine))
         {
            return newMachine.Execute().Optional();
         }
         else
         {
            return _newMachine.Exception;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public override string ToString() => "execute.string";
}