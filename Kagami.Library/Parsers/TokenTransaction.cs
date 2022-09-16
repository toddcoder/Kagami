using System.Collections.Generic;

namespace Kagami.Library.Parsers
{
   public class TokenTransaction
   {
      protected List<Token> tokens;
      protected Stack<int> lengths;

      public TokenTransaction(List<Token> tokens)
      {
         this.tokens = tokens;
         lengths = new Stack<int>();
      }

      public void Begin() => lengths.Push(tokens.Count);

      public void RollBack()
      {
         var index = lengths.Pop();
         tokens.RemoveRange(index, tokens.Count - index);
      }

      public void Commit() => lengths.Pop();
   }
}