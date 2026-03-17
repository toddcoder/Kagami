using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class Operations
{
   protected List<Operation> operations;
   protected int address;
   protected int length;

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

   public void Append(Operations newOperations)
   {
      operations.AddRange(newOperations.operations);
      length = operations.Count;
   }

   public void AppendStop()
   {
      operations.Add(new Stop());
      length = operations.Count;
   }

   public int Count => length;

   public override string ToString()
   {
      var table = new TableMaker(("Loc", Justification.Right), ("Operation", Justification.Left));
      for (var i = 0; i < length; i++)
      {
         table.Add(i, operations[i]);
      }

      return table.ToString();
   }
}