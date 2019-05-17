using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
	public class SendBindingMessageSymbol : SendMessageSymbol
	{
		public SendBindingMessageSymbol(Selector selector, IMaybe<LambdaSymbol> lambda, IMaybe<Operation> operation,
			params Expression[] arguments) :
			base(selector, Precedence.SendMessage, lambda, operation, arguments) { }

		public SendBindingMessageSymbol(Selector selector, params Expression[] arguments) :
			base(selector, Precedence.SendMessage, arguments) { }

		public SendBindingMessageSymbol(Selector selector, IMaybe<Operation> operation, params Expression[] arguments) :
			base(selector, Precedence.SendMessage, operation, arguments) { }

		public SendBindingMessageSymbol(Selector selector, IMaybe<LambdaSymbol> lambda, params Expression[] arguments) :
			base(selector, Precedence.SendMessage, lambda, arguments) { }

		public override void Generate(OperationsBuilder builder)
		{
			var endLabel = newLabel("end");

			builder.Dup();
			builder.SendMessage("canBind".get(), 0);
			builder.GoToIfFalse(endLabel);
			builder.SendMessage("value".get(), 0);

			base.Generate(builder);

			builder.Label(endLabel);
			builder.NoOp();
		}

		public override Precedence Precedence => Precedence.SendMessage;

		public override string ToString() => $"?.{selector.Image}({arguments.Listify()})";
	}
}