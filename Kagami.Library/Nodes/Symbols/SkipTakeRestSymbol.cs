using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class SkipTakeRestSymbol : Symbol
	{
		public override void Generate(OperationsBuilder builder) => builder.SendMessage("takeRest()");

		public override Precedence Precedence => Precedence.SendMessage;

		public override Arity Arity => Arity.Postfix;
	}
}