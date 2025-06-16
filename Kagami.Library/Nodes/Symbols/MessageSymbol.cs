using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Enumerables;
using Core.Monads;

namespace Kagami.Library.Nodes.Symbols;

public class MessageSymbol : Symbol
{
   protected Selector selector;
   protected Expression[] arguments;
   protected Maybe<LambdaSymbol> _lambda;

   public MessageSymbol(Selector selector, Expression[] arguments, Maybe<LambdaSymbol> _lambda)
   {
      this.selector = selector;
      this.arguments = arguments;
      this._lambda = _lambda;
   }

   public override void Generate(OperationsBuilder builder)
   {
      foreach (var argument in arguments)
      {
         argument.Generate(builder);
      }

      int count;
      if (_lambda is (true, var lambda))
      {
         lambda.Generate(builder);
         count = arguments.Length + 1;
      }
      else
      {
         count = arguments.Length;
      }

      builder.Peek(Index);
      builder.NewMessage(selector, count);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString()
   {
      return $"?{selector}({arguments.Select(a => a.ToString()).ToString(", ")})" + (_lambda.Map(l => $" {l}") | (() => ""));
   }
}