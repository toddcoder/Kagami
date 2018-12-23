using System.Collections.Generic;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Enumerables;
using Standard.Types.Monads;
using static Standard.Types.Monads.AttemptFunctions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SymbolStack
   {
      Stack<Symbol> stack;

      public SymbolStack() => stack = new Stack<Symbol>();

      public void Push(Symbol symbol) => stack.Push(symbol);

      public IResult<Symbol> Pop() => tryTo(() => stack.Pop());

      public IMaybe<Symbol> Peek() => when(!IsEmpty, () => stack.Peek());

      public bool IsEmpty => stack.Count == 0;

      public bool IsPending(Symbol next)
      {
         if (IsEmpty)
            return false;

         var symbol = stack.Peek();
         if (!symbol.LeftToRight)
            return symbol.Precedence < next.Precedence;

         return symbol.Precedence <= next.Precedence;
      }

      public void Clear() => stack.Clear();

      public override string ToString() => stack.Listify(" ");
   }
}