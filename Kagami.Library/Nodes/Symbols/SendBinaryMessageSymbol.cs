using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Standard.Types.Strings;

namespace Kagami.Library.Nodes.Symbols
{
	public class SendBinaryMessageSymbol : Symbol
	{
		Selector selector;
		Precedence precedence;
		bool swap;
		string label;

		public SendBinaryMessageSymbol(Selector selector, Precedence precedence, bool swap = false, string label = "")
		{
			this.selector = selector;
			this.precedence = precedence;
			this.swap = swap;
			this.label = label;
		}

		public override void Generate(OperationsBuilder builder)
		{
			if (swap)
				builder.Swap();
			if (label.IsNotEmpty())
				builder.ArgumentLabel(label);
			builder.SendMessage(selector, 1);
		}

		public override Precedence Precedence => precedence;

		public override Arity Arity => Arity.Binary;

		public override string ToString() => selector.Image;
	}
}