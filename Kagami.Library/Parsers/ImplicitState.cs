using Kagami.Library.Nodes.Symbols;

namespace Kagami.Library.Parsers
{
	public class ImplicitState
	{
		public ImplicitState(Symbol symbol, string message, int parameterCount)
		{
			Symbol = symbol;
			Message = message;
			ParameterCount = parameterCount;
		}

		public Symbol Symbol { get; }

		public string Message { get; }

		public int ParameterCount { get; }
	}
}