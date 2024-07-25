using System.Collections.Generic;

namespace Kagami.Library.Parsers
{
   public class IndexTransaction
   {
      protected Stack<int> stack;

      public IndexTransaction() => stack = new Stack<int>();

      public void Begin(int index) => stack.Push(index);

      public int RollBack() => stack.Pop();

      public void Commit() => stack.Pop();
   }
}