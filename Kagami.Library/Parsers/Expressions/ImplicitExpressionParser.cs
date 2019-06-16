using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class ImplicitExpressionParser : EndingInExpressionParser
	{
		string message;
		int parameterCount;
		string fieldName;

		public ImplicitExpressionParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => $"^ /(|s|) /({REGEX_ITERATOR_FUNCTIONS}) /'('";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			message = tokens[2].Text;
			state.Colorize(tokens, Color.Whitespace, Color.CollectionPart, Color.OpenParenthesis);

			parameterCount = 1;
			fieldName = "__$0";
			switch (message)
			{
				case "acc":
					message = "foldl";
					goto case "foldl";
				case "foldl":
					message += "(_,_)";
					fieldName = "__$1";
					parameterCount = 2;
					break;
				case "reducel":
					message += "(_,_)";
					fieldName = "__$1";
					parameterCount = 2;
					break;
				case "accr":
					message = "foldr";
					goto case "foldr";
				case "foldr":
				case "reducer":
					message += "(_,_)";
					parameterCount = 2;
					break;
				case "while":
					message = "takeWhile(_,_)";
					break;
				case "until":
					message = "takeUntil(_,_)";
					break;
				case "zip":
				case "z":
				case "cross":
				case "x":
					parameterCount = 2;
					message = message == "z" || message == "zip" ? "zip(_,_,_)" : "cross(_,_,_)";
					break;
				default:
					message += "(_,_)";
					break;
			}

			state.BeginImplicitExpressionState();
			state.ImplicitExpressionState.FieldName1 = fieldName;
			if (parameterCount == 2)
				state.ImplicitExpressionState.FieldName2 = fieldName == "__$0" ? "__$1" : "__$0";

			return Unit.Matched();
		}

		public override IMatched<Unit> Suffix(ParseState state, Expression expression)
		{
			state.Scan("^ /(|s|) /')'", Color.Whitespace, Color.CloseParenthesis);

			var implicitExpressionState = state.ImplicitExpressionState;
			state.EndImplicitExpressionState();
			if (implicitExpressionState.Symbol1.If(out var symbol))
			{
				var implicitExpressionSymbol =
					new ImplicitExpressionSymbol(expression, message, parameterCount, symbol, implicitExpressionState.Symbol2);
				builder.Add(implicitExpressionSymbol);

				return Unit.Matched();
			}
			else
				return "Collection or iterable not expressed".FailedMatch<Unit>();
		}
	}
}