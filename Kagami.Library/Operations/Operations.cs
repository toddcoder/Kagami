using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class Operations
   {
      Operation[] operations;
      int address;
      int length;

      public Operations(Operation[] operations)
      {
         this.operations = operations;
         address = 0;
         length = operations.Length;
      }

      public Operations()
         : this(new Operation[0]) { }

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

      public IMaybe<Operation> Current => when(address.Between(0).Until(length), () => operations[address]);

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
}