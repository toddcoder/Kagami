using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;
using Core.Enumerables;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class Expression : Symbol
{
   public static explicit operator Block(Expression expression) => new(new ExpressionStatement(expression, true));

   public static Expression FromSymbol(Symbol symbol) => new(symbol);

   public static Expression Empty => new();

   protected Symbol[] symbols;

   public Expression(Symbol[] symbols)
   {
      this.symbols = symbols;
   }

   public Expression(Symbol symbol)
   {
      symbols = [symbol];
   }

   protected Expression()
   {
      symbols = [];
   }

   public Symbol[] Symbols => symbols;

   public override void Generate(OperationsBuilder builder)
   {
      foreach (var symbol in symbols)
      {
         symbol.Generate(builder);
      }
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => symbols.ToString(" ");

   public void Replace(Predicate<Symbol> predicate, Func<Symbol, Symbol> replacement)
   {
      for (var i = 0; i < symbols.Length; i++)
      {
         if (predicate(symbols[i]))
         {
            symbols[i] = replacement(symbols[i]);
         }
      }
   }

   public int SpecialComparisandIndex { get; set; } = -1;

   public SelectorItem SelectorItem()
   {
      var symbol = symbols[0];
      if (symbol is NameValueSymbol nameValue)
      {
         var (name, _) = nameValue;
         return new SelectorItem(name, nil, SelectorItemType.Normal);
      }
      else
      {
         return new SelectorItem("", nil, SelectorItemType.Normal);
      }
   }
}