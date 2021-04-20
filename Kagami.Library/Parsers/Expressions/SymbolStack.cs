using System.Collections.Generic;
using Kagami.Library.Nodes.Symbols;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SymbolStack
   {
      protected Stack<Symbol> stack;

      public SymbolStack() => stack = new Stack<Symbol>();

      public void Push(Symbol symbol) => stack.Push(symbol);

      public IResult<Symbol> Pop() => tryTo(() => stack.Pop());

      public IMaybe<Symbol> Peek() => maybe(!IsEmpty, () => stack.Peek());

      public bool IsEmpty => stack.Count == 0;

      public bool IsPending(Symbol next)
      {
         if (IsEmpty)
         {
            return false;
         }

         var symbol = stack.Peek();
         if (!symbol.LeftToRight)
         {
            return symbol.Precedence < next.Precedence;
         }

         return symbol.Precedence <= next.Precedence;
      }

      public void Clear() => stack.Clear();

      public override string ToString() => stack.ToString(" ");
   }
}