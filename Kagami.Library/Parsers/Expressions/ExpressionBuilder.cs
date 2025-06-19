using Kagami.Library.Nodes.Symbols;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ExpressionBuilder(Bits32<ExpressionFlags> flags, bool acknowlegeImplicit = true)
{
   protected SymbolStack stack = new();
   protected List<Symbol> symbols = [];
   protected List<Symbol> ordered = [];
   protected Bits32<ExpressionFlags> flags = flags;
   protected Maybe<Symbol> _lastSymbol = nil;
   protected bool containsImplicitOperator;

   public Bits32<ExpressionFlags> Flags
   {
      get => flags;
      set => flags = value;
   }

   public Symbol[] Symbols
   {
      get => [.. symbols];
      set => symbols = value.ToList();
   }

   public void Add(Symbol symbol)
   {
      if (acknowlegeImplicit && symbol is ImplicitSymbol)
      {
         containsImplicitOperator = true;
      }

      ordered.Add(symbol);
      _lastSymbol = symbol;

      while (stack.IsPending(symbol))
      {
         if (stack.Pop() is (true, var poppedSymbol))
         {
            symbols.Add(poppedSymbol);

            if (poppedSymbol is ISpecialComparisand)
            {
               SpecialComparisandIndex = symbols.Count - 1;
            }
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

         if (symbol is ISpecialComparisand)
         {
            SpecialComparisandIndex = symbols.Count - 1;
         }
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

            if (symbol is ISpecialComparisand)
            {
               SpecialComparisandIndex = symbols.Count - 1;
            }
         }
         else
         {
            return _symbol.Exception;
         }
      }

      return unit;
   }

   protected static Result<Expression> generateMap(Expression originalExpression, ExpressionFlags flags)
   {
      var symbols = originalExpression.Symbols;
      var _index = symbols.Find(s => s is ImplicitSymbol);
      if (_index is (true, var index))
      {
         var sourceSymbol = symbols[index - 1];
         symbols[index - 1] = new FieldSymbol("__$0");
         symbols[index] = new NoOpSymbol();
         var bodyExpression = new Expression(symbols);
         var block = new Block(bodyExpression);
         var lambda = new LambdaSymbol(1, block);

         var builder = new ExpressionBuilder(flags, false);
         builder.Add(sourceSymbol);
         builder.Add(new SendMessageSymbol("map(_)", lambda));

         return builder.ToExpression();
      }
      else
      {
         return originalExpression;
      }
   }

   public Result<Expression> ToExpression()
   {
      var _expression = EndOfExpression().Map(_ => new Expression([.. symbols]) { SpecialComparisandIndex = SpecialComparisandIndex });
      if (containsImplicitOperator && _expression is (true, var expression))
      {
         return generateMap(expression, flags);
      }

      return _expression;
   }

   public Symbol[] Ordered
   {
      get => [.. ordered];
      set => ordered = value.ToList();
   }

   public override string ToString() => ordered.ToString(" ");

   public int Length => ordered.Count;

   public void Clear()
   {
      stack.Clear();
      symbols.Clear();
      ordered.Clear();
   }

   public Maybe<Symbol> LastSymbol => _lastSymbol;

   public int SpecialComparisandIndex { get; set; } = -1;
}