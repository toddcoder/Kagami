﻿using Kagami.Library.Nodes.Symbols;

namespace Kagami.Library.Parsers
{
	public class ImplicitState
	{
		public ImplicitState(Symbol symbol, string message, int parameterCount, string fieldName)
		{
			Symbol = symbol;
			Message = message;
			ParameterCount = parameterCount;
			FieldName = fieldName;
		}

		public Symbol Symbol { get; }

		public string Message { get; }

		public int ParameterCount { get; }

		public string FieldName { get; }

		public string Left => FieldName;

		public string Right => FieldName == "__$0" ? "__$1" : "__$0";
	}
}