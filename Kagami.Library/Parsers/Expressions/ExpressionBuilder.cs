using Kagami.Library.Nodes.Symbols;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ExpressionBuilder(Bits32<ExpressionFlags> flags)
{
   protected SymbolStack stack = new();
   protected List<Symbol> symbols = [];
   protected List<Symbol> ordered = [];
   protected Bits32<ExpressionFlags> flags = flags;
   protected Maybe<Symbol> _lastSymbol = nil;

   public Bits32<ExpressionFlags> Flags
   {
      get => flags;
      set => flags = value;
   }

   public void Add(Symbol symbol)
   {
      ordered.Add(symbol);
      _lastSymbol = symbol;

      while (stack.IsPending(symbol))
      {
         if (stack.Pop() is (true, var poppedSymbol))
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

   public Result<Unit> EndOfExpression()
   {
      while (!stack.IsEmpty)
      {
         var _symbol = stack.Pop();
         if (_symbol is (true, var symbol))
         {
            symbols.Add(symbol);
         }
         else
         {
            return _symbol.Exception;
         }
      }

      return unit;
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

   public Maybe<Symbol> LastSymbol => _lastSymbol;
}