using System.Linq;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class ImplicitMessageParser : SymbolParser
	{
		static string parameters(int count)
		{
			return $"({Enumerable.Range(0, count).Select(i => "_").Stringify(",")})";
		}

		public ImplicitMessageParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /('sort' | 'foldl' | 'foldr' | 'reducel' | 'reducer' | " +
			"'count' | 'map' | 'flatMap' | 'bind' | 'if' | 'ifNot' | 'index' | 'min' | 'max' | 'first' | " +
			"'last' | 'split' | 'one' | 'none' | 'any' | 'all' | 'span' | 'groupBy' | 'each') /'.'";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var message = tokens[2].Text;
			state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Operator);

			if (getValue(state, builder.Flags).Out(out var symbol, out var original))
			{
				var parameterCount = 1;
				var fieldName = "__$0";
				switch (message)
				{
					case "foldl":
						fieldName = "__$1";
						parameterCount = 2;
						break;
					case "reducel":
						fieldName = "__$1";
						parameterCount = 2;
						break;
					case "foldr":
					case "reducer":
						parameterCount = 2;
						break;
				}

				state.ImplicitState = new ImplicitState(symbol, message + parameters(parameterCount), parameterCount, fieldName).Some();
				builder.Add(new FieldSymbol(fieldName));

				return Unit.Matched();
			}
			else
				return original.Unmatched<Unit>();
		}
	}
}