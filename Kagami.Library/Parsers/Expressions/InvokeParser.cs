using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class InvokeParser : SymbolParser
	{
		public InvokeParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => $"^ /(|s|) /({REGEX_FUNCTION_NAME}) /'('";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var functionName = tokens[2].Text;
			if (functionName == @"\/")
				return notMatched<Unit>();
			else
			{
				state.Colorize(tokens, Color.Whitespace, Color.Invokable, Color.Structure);

				if (getArgumentsPlusLambda(state, builder.Flags).Out(out var tuple, out var original))
				{
					var (arguments, possibleLambda) = tuple;

					if (state.BlockFollows())
					{
						state.Scan("^ /':'", Color.Structure);
						if (state.Advance().Out(out _, out var unitMatched))
						{
							var tempObjectField = newLabel("object");
							var outerBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
							var setPropertyParser = new SetPropertyParser(builder, tempObjectField, outerBuilder);
							while (state.More)
								if (setPropertyParser.Scan(state).If(out _, out var mbException)) { }
								else if (mbException.If(out var exception))
									return failedMatch<Unit>(exception);
								else
									break;

							state.Regress();

							if (outerBuilder.ToExpression().If(out var outerExpression, out var outerException))
								builder.Add(new NewObjectSymbol(tempObjectField, functionName, outerExpression));
							else
								return failedMatch<Unit>(outerException);
						}
						else
							return unitMatched;
					}
					else if (state.Macro(functionName).If(out var function))
						builder.Add(new MacroInvokeSymbol(function, arguments, possibleLambda));
					else
						builder.Add(new InvokeSymbol(functionName, arguments, possibleLambda));

					return Unit.Matched();
				}
				else
					return original.Unmatched<Unit>();
			}
		}
	}
}