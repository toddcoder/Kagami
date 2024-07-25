using System.Collections.Generic;
using Kagami.Library.Nodes.Symbols;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class ExpressionBuilder
   {
      protected SymbolStack stack;
      protected List<Symbol> symbols;
      protected List<Symbol> ordered;
      protected Bits32<ExpressionFlags> flags;
      protected IMaybe<Symbol> _lastSymbol;

      public ExpressionBuilder(Bits32<ExpressionFlags> flags)
      {
         stack = new SymbolStack();
         symbols = new List<Symbol>();
         ordered = new List<Symbol>();
         this.flags = flags;
         _lastSymbol = none<Symbol>();
      }

      public Bits32<ExpressionFlags> Flags
      {
         get => flags;
         set => flags = value;
      }

      public void Add(Symbol symbol)
      {
         ordered.Add(symbol);
         _lastSymbol = symbol.Some();

         while (stack.IsPending(symbol))
         {
            if (stack.Pop().If(out var poppedSymbol))
            {
               symbols.Add(poppedSymbol);
            }
            else
            {
               return;
            }
         }

         if (symbol.Precedence != Precedence.Value)
         {
            stack.Push(symbol);
         }
         else
         {
            symbols.Add(symbol);
         }
      }

      public IResult<Unit> EndOfExpression()
      {
         while (!stack.IsEmpty)
         {
            if (stack.Pop().If(out var symbol, out var exception))
            {
               symbols.Add(symbol);
            }
            else
            {
               return failure<Unit>(exception);
            }
         }

         return Unit.Success();
      }

      public Result<Expression> ToExpression() => EndOfExpression().Map(_ => new Expression(symbols.ToArray()));

      public IEnumerable<Symbol> Ordered => ordered;

      public override string ToString() => ordered.ToString(" ");

      public int Length => ordered.Count;

      public void Clear()
      {
         stack.Clear();
         symbols.Clear();
         ordered.Clear();
      }

      public IMaybe<Symbol> LastSymbol => _lastSymbol;
   }
}