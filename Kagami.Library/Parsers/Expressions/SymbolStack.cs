using Core.DataStructures;
using Kagami.Library.Nodes.Symbols;
using Core.Enumerables;
using Core.Monads;

namespace Kagami.Library.Parsers.Expressions;

public class SymbolStack
{
   protected MaybeStack<Symbol> stack = [];

   public void Push(Symbol symbol) => stack.Push(symbol);

   public Result<Symbol> Pop() => stack.Pop().Result("Stack is empty");

   public Maybe<Symbol> Peek() => stack.Peek();

   public bool IsEmpty => stack.Count == 0;

   public bool IsPending(Symbol next)
   {
      if (IsEmpty)
      {
         return false;
      }

      if (stack.Peek() is (true, var symbol))
      {
         if (!symbol.LeftToRight)
         {
            return symbol.Precedence < next.Precedence;
         }

         return symbol.Precedence <= next.Precedence;
      }
      else
      {
         return false;
      }
   }

   public void Clear() => stack.Clear();

   public override string ToString() => stack.ToString(" ");
}