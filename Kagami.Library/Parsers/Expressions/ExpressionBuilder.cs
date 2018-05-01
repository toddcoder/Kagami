using System.Collections.Generic;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using Standard.Types.Numbers;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class ExpressionBuilder
   {
      SymbolStack stack;
      List<Symbol> symbols;
      List<Symbol> ordered;
      Bits32<ExpressionFlags> flags;
      IMaybe<Symbol> lastSymbol;

      public ExpressionBuilder(Bits32<ExpressionFlags> flags)
      {
         stack = new SymbolStack();
         symbols = new List<Symbol>();
         ordered = new List<Symbol>();
         this.flags = flags;
         lastSymbol = none<Symbol>();
      }

      public Bits32<ExpressionFlags> Flags
      {
         get => flags;
         set => flags = value;
      }

      public IResult<Unit> Add(Symbol symbol)
      {
         ordered.Add(symbol);
         lastSymbol = symbol.Some();

         while (stack.IsPending(symbol))
            if (stack.Pop().If(out var poppedSymbol, out var exception))
               symbols.Add(poppedSymbol);
            else
               return failure<Unit>(exception);

         if (symbol.Precedence != Precedence.Value)
         {
            stack.Push(symbol);
            return Unit.Success();
         }
         else
         {
            symbols.Add(symbol);
            return Unit.Success();
         }
      }

      public IResult<Unit> EndOfExpression()
      {
         while (!stack.IsEmpty)
            if (stack.Pop().If(out var symbol, out var exception))
               symbols.Add(symbol);
            else
               return failure<Unit>(exception);

         return Unit.Success();
      }

      public IResult<Expression> ToExpression() => EndOfExpression().Map(u => new Expression(symbols.ToArray()));

      public IEnumerable<Symbol> Ordered => ordered;

      public override string ToString() => ordered.Listify(" ");

      public int Length => ordered.Count;

      public void Clear()
      {
         stack.Clear();
         symbols.Clear();
         ordered.Clear();
      }

      public IMaybe<Symbol> LastSymbol => lastSymbol;
   }
}