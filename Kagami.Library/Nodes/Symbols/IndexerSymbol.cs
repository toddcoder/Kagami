using Core.Arrays;
using Kagami.Library.Operations;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class IndexerSymbol : Symbol, IHasExpressions
{
   public static void Get(OperationsBuilder builder, Expression[] arguments)
   {
      var _symbol = GetIndex(builder, arguments);
      if (_symbol is (true, OpenRangeSymbol))
      {
         Selector selector = "fromOpenRange(_)";
         builder.SendMessage(selector, 1);
      }
      else
      {
         Selector selector = "[](_)";
         builder.SendMessage(selector, 1);
      }
   }

   public static Maybe<Symbol> GetIndex(OperationsBuilder builder, Expression[] arguments)
   {
      Maybe<Symbol> _symbol = nil;
      foreach (var expression in arguments)
      {
         expression.Generate(builder);
         if (!_symbol && expression.Symbols.Last() is (true, OpenRangeSymbol openRange))
         {
            _symbol = openRange;
         }
      }

      return _symbol;
   }

   protected Expression[] arguments;

   public IndexerSymbol(Expression[] arguments)
   {
      this.arguments = arguments;
   }

   public override Precedence Precedence => Precedence.SendMessage;

   public override Arity Arity => Arity.Postfix;

   public override void Generate(OperationsBuilder builder)
   {
      var newSequenceSymbol = new NewSequenceSymbol(arguments);
      newSequenceSymbol.Generate(builder);
      var isOpenRange = newSequenceSymbol.IsOpenRange;
      if (isOpenRange)
      {
         Selector selector = "fromOpenRange(_)";
         builder.SendMessage(selector, 1);
      }
      else
      {
         Selector selector = "[](_)";
         builder.SendMessage(selector, 1);
      }
   }

   public override string ToString() => $"[{arguments.ToString(", ")}]";

   public Expression[] Expressions => arguments;
}