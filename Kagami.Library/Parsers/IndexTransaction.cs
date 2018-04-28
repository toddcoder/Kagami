using System.Collections.Generic;

namespace Kagami.Library.Parsers
{
   public class IndexTransaction
   {
      Stack<int> stack;

      public IndexTransaction() => stack = new Stack<int>();

      public void Begin(int index) => stack.Push(index);

      public int RollBack() => stack.Pop();

      public void Commit() => stack.Pop();
   }
}