using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class ConversionSymbol : Symbol
	{
		string message;
		Symbol value;

		public ConversionSymbol(string message, Symbol value)
		{
			this.message = message;
			this.value = value;
		}

		public override void Generate(OperationsBuilder builder)
		{
			builder.GetField("math");
			value.Generate(builder);
			builder.SendMessage($"{message}(_)", 1);
		}

		public override Precedence Precedence => Precedence.Value;

		public override Arity Arity => Arity.Nullary;

		public override string ToString() => $"{message} {value}";
	}
}