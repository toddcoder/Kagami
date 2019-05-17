using System;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;
using Core.Enumerables;

namespace Kagami.Library.Nodes.Symbols
{
   public class Expression : Symbol
   {
      public static explicit operator Block(Expression expression) => new Block(new ExpressionStatement(expression, true));

      Symbol[] symbols;

      public Expression(Symbol[] symbols) => this.symbols = symbols;

      public Expression(Symbol symbol) => symbols = new[] { symbol };

      public Symbol[] Symbols => symbols;

      public override void Generate(OperationsBuilder builder)
      {
         foreach (var symbol in symbols)
            symbol.Generate(builder);
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => symbols.Join(" ");

      public void Replace(Predicate<Symbol> predicate, Func<Symbol, Symbol> replacement)
      {
         for (var i = 0; i < symbols.Length; i++)
            if (predicate(symbols[i]))
               symbols[i] = replacement(symbols[i]);
      }
   }
}