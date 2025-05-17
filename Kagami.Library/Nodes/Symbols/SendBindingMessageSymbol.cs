using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class SendBindingMessageSymbol : SendMessageSymbol
{
   public SendBindingMessageSymbol(Selector selector, Maybe<LambdaSymbol> _lambda, Maybe<Operation> _operation,
      params Expression[] arguments) : base(selector, _lambda, _operation, arguments)
   {
   }

   public SendBindingMessageSymbol(Selector selector, params Expression[] arguments) : base(selector, arguments)
   {
   }

   public SendBindingMessageSymbol(Selector selector, Maybe<Operation> operation, params Expression[] arguments) :
      base(selector, operation, arguments)
   {
   }

   public SendBindingMessageSymbol(Selector selector, Maybe<LambdaSymbol> lambda, params Expression[] arguments) :
      base(selector, lambda, arguments)
   {
   }

   public override void Generate(OperationsBuilder builder)
   {
      var endLabel = newLabel("end");

      builder.Dup();
      builder.SendMessage("canBind".get(), 0);
      builder.GoToIfFalse(endLabel);
      builder.Dup();
      builder.SendMessage("value".get(), 0);

      base.Generate(builder);

      builder.SendMessage("unit(_)", 1);

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.SendMessage;

   public override string ToString() => $"?.{selector.Image}({arguments.ToString(", ")})";
}