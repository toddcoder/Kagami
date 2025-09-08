using Kagami.Library.Nodes.Symbols;
using Core.Enumerables;
using Core.Monads;
using Core.Monads.Lazy;
using Core.Numbers;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ExpressionBuilder(Bits32<ExpressionFlags> flags, bool acknowledgeImplicit = true)
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
      if (acknowledgeImplicit && symbol is ImplicitSymbol or ImplicitZip or ImplicitFold or ImplicitMapSymbol)
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
      LazyMaybe<int> _implicitMapIndex = nil;
      LazyMaybe<int> _zipIndex1 = nil;
      LazyMaybe<int> _foldIndex = nil;
      var _index = symbols.Find(s => s is ImplicitSymbol);
      if (_index is (true, var index))
      {
         var implicitType = ((ImplicitSymbol)symbols[index]).Type;
         var sourceSymbol = symbols[index + 1];
         symbols[index + 1] = new FieldSymbol("__$0");
         symbols[index] = new NoOpSymbol();

         var bodyExpression = new Expression(symbols);
         var block = new Block(bodyExpression);
         var lambda = new LambdaSymbol(1, block);

         var builder = new ExpressionBuilder(flags, false);
         builder.Add(sourceSymbol);
         Selector selector = implicitType switch
         {
            "i" => "if(_)",
            "e" => "each(_)",
            _ => "map(_)"
         };
         builder.Add(new SendMessageSymbol(selector, Precedence.ChainedOperator, false, lambda));

         return builder.ToExpression();
      }
      else if (_implicitMapIndex.ValueOf(symbols.Find(s => s is ImplicitMapSymbol)) is (true, var implicitMapIndex))
      {
         var sourceSymbol = symbols[implicitMapIndex - 1];
         symbols[implicitMapIndex - 1] = new FieldSymbol("__$0");
         symbols[implicitMapIndex] = new NoOpSymbol();

         var bodyExpression = new Expression(symbols);
         var block = new Block(bodyExpression);
         var lambda = new LambdaSymbol(1, block);

         var builder = new ExpressionBuilder(flags, false);
         builder.Add(sourceSymbol);
         builder.Add(new SendMessageSymbol("map(_)", Precedence.ChainedOperator, false, lambda));

         return builder.ToExpression();
      }
      else if (_zipIndex1.ValueOf(symbols.Find(s => s is ImplicitZip)) is (true, var zipIndex1))
      {
         var _zipIndex2 = symbols.Find(s => s is ImplicitZip, zipIndex1 + 1);
         if (_zipIndex2 is (true, var zipIndex2))
         {
            var sourceSymbol1 = symbols[zipIndex1 + 1];
            symbols[zipIndex1 + 1] = new FieldSymbol("__$0");
            symbols[zipIndex1] = new NoOpSymbol();
            var sourceSymbol2 = symbols[zipIndex2 + 1];
            symbols[zipIndex2 + 1] = new FieldSymbol("__$1");
            symbols[zipIndex2] = new NoOpSymbol();

            var bodyExpression = new Expression(symbols);
            var block = new Block(bodyExpression);
            var lambda = new LambdaSymbol(2, block);

            var builder = new ExpressionBuilder(flags, false);
            builder.Add(sourceSymbol1);
            builder.Add(new SendMessageSymbol("zip(_,_)", Precedence.ChainedOperator, false, lambda, new Expression(sourceSymbol2)));

            return builder.ToExpression();
         }
         else
         {
            return originalExpression;
         }
      }
      else if (_foldIndex.ValueOf(symbols.Find(s => s is ImplicitFold)) is (true, var foldIndex))
      {
         var _foldVariableIndex = symbols.Find(s => s is ImplicitFoldVariable);
         if (_foldVariableIndex is (true, var foldVariableIndex))
         {
            var sourceSymbol1 = symbols[foldIndex + 1];
            symbols[foldIndex + 1] = new FieldSymbol("__$0");
            symbols[foldIndex] = new NoOpSymbol();

            var sourceSymbol2 = symbols[foldVariableIndex + 1];
            symbols[foldVariableIndex + 1] = sourceSymbol2;
            symbols[foldVariableIndex] = new NoOpSymbol();

            var bodyExpression = new Expression(symbols);
            var block = new Block(bodyExpression);
            var parameter1 = new Parameter(false, "", "__$0", nil, nil, false, false);
            Parameter parameter2;
            if (sourceSymbol2 is FieldSymbol fieldSymbol)
            {
               parameter2 = new Parameter(false, "", fieldSymbol.FieldName, nil, nil, false, false);
            }
            else
            {
               parameter2 = new Parameter(false, "", "__$1", nil, nil, false, false);
            }

            var parameters = new Parameters(parameter1, parameter2);
            var lambda = new LambdaSymbol(parameters, block);

            var builder = new ExpressionBuilder(flags, false);
            builder.Add(sourceSymbol1);
            builder.Add(new SendMessageSymbol("foldl(_)", Precedence.ChainedOperator, false, lambda));

            return builder.ToExpression();
         }
         else
         {
            return originalExpression;
         }
      }
      else
      {
         return originalExpression;
      }
   }

   public Result<Expression> ToExpression(bool clear = false)
   {
      var _expression = EndOfExpression().Map(_ => new Expression([.. symbols]) { SpecialComparisandIndex = SpecialComparisandIndex });
      if (containsImplicitOperator && _expression is (true, var expression))
      {
         if (clear)
         {
            Clear();
         }

         return generateMap(expression, flags);
      }

      if (clear)
      {
         Clear();
      }

      return _expression;
   }

   public ExpressionBuilder Subexpression()
   {
      var builder = new ExpressionBuilder(flags, acknowledgeImplicit);
      if (ToExpression() is (true, var oldExpression))
      {
         builder.Add(new SubexpressionSymbol(oldExpression));
      }

      return builder;
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