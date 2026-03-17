using Core.Collections;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class Operations
{
   protected readonly List<Operation> operations;
   protected int address;
   protected int length;
   protected Memo<string, Operations> runtimeOperations = new Memo<string, Operations>.Function(createOperationsFromExpression);

   public Operations(IEnumerable<Operation> operations)
   {
      this.operations = [.. operations];
      address = 0;
      length = this.operations.Count;
   }

   public Operations() : this([])
   {
   }

   public int Address => address;

   public bool Goto(int address)
   {
      if (address.Between(0).Until(length))
      {
         this.address = address;
         return true;
      }

      return false;
   }

   public void Advance(int increment) => address += increment;

   public bool More => address < length;

   public Operation this[int index] => operations[index];

   public Maybe<Operation> Current => maybe<Operation>() & address.Between(0).Until(length) & (() => operations[address]);

   public void GoPastEnd() => address = length;

   public override string ToString()
   {
      var table = new TableMaker(("Loc", Justification.Right), ("Operation", Justification.Left));
      for (var i = 0; i < length; i++)
      {
         table.Add(i, operations[i]);
      }

      return table.ToString();
   }

   public void Append(Operation operation) => operations.Add(operation);

   public int Append(Operations newOperations)
   {
      var index = length;
      operations.AddRange(newOperations.operations);
      length = operations.Count;

      return index;
   }

   public Optional<Lambda> CreateOperationsFromExpression(string expressionSource, int returnIndex)
   {
      try
      {
         var newOperations = runtimeOperations[expressionSource];
         var index = operations.Count;
         Append(newOperations);
         var 
         /*var index = operations.Count;
         if (newOperations.operations[^1] is not GoTo)
         {
            newOperations.Append(operation);
            Append(new Stop());
            Append(newOperations);
         }

         return new GoTo { Address = index };*/
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected static Operations createOperationsFromExpression(string expressionSource)
   {
      var compiler = new Compiler(expressionSource, new CompilerConfiguration(), Machine.Current.Value.Context);
      var _result = compiler.Generate();
      if (_result)
      {
         var statements = compiler.Statements;
         if (statements.Length > 0 && statements[0] is ExpressionStatement)
         {
            var operations = compiler.Operations.Required("Operations not generated");
            operations.Append(new Return(true));

            return compiler.Operations;
         }
         else
         {
            throw fail($"{expressionSource} can't be generated");
         }
      }
      else
      {
         throw _result.Exception;
      }
   }
}