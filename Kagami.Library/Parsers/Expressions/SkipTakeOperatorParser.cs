using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class SkipTakeOperatorParser : SymbolParser
	{
		public SkipTakeOperatorParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /'{'";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Structure);

			var first = true;

			while (state.More)
			{
				var ((matched, _), (failed, exception)) = getItem(state, builder, first);
				if (matched)
				{
					if (state.Scan("^ /(|s|) /[',}']", Color.Whitespace, Color.Structure).If(out var found, out var anyException))
					{
						first = false;
						if (found.Contains("}"))
						{
							builder.Add(new SendMessageSymbol("fullResult".get(), Precedence.SendMessage));
							return Unit.Matched();
						}
					}
					else if (anyException.If(out exception))
					{
						return failedMatch<Unit>(exception);
					}
					else
					{
						return "Expected , or }".FailedMatch<Unit>();
					}
				}
				else if (failed)
				{
					return failedMatch<Unit>(exception);
				}
				else
				{
					return notMatched<Unit>();
				}
			}

			return "Open braces".FailedMatch<Unit>();
		}

		static IMatched<Unit> getItem(ParseState state, ExpressionBuilder builder, bool first)
		{
			var result =
				from literal in getLiteral(state, builder, first)
				from skipTake in getSkipTake(literal, state, builder, first)
				select literal || skipTake;

			if (result.If(out var found, out var exception))
			{
				return found ? Unit.Matched() : "Expected expression".FailedMatch<Unit>();
			}
			else
			{
				return failedMatch<Unit>(exception);
			}
		}

		static IResult<bool> getLiteral(ParseState state, ExpressionBuilder builder, bool first)
		{
			var ((matched, _), (failed, exception)) = state.Scan("^ /(|s|) /'='", Color.Whitespace, Color.Structure);
			if (matched)
			{
				if (getExpression(state, ExpressionFlags.OmitComma).If(out var expression, out var anyException))
				{
					if (first)
					{
						builder.Add(new SkipTakeInitLiteralSymbol(expression));
					}
					else
					{
						builder.Add(new SkipTakeLiteralSymbol(expression));
					}

					return true.Success();
				}
				else if (anyException.If(out exception))
				{
					return failure<bool>(exception);
				}
				else
				{
					return "Expected expression".Failure<bool>();
				}
			}
			else if (failed)
			{
				return failure<bool>(exception);
			}
			else
			{
				return failed.Success();
			}
		}

		static IResult<bool> getSkipTake(bool literalFound, ParseState state, ExpressionBuilder builder, bool first)
		{
			if (literalFound)
			{
				return false.Success();
			}
			else if (state.Scan("^ /(|s|) /';'", Color.Whitespace, Color.Structure).IsMatched)
			{
				if (state.Scan("^ /(|s|) /'*'", Color.Whitespace, Color.Structure).IsMatched)
				{
					if (first)
					{
						builder.Add(new SkipTakeRestInitSymbol());
					}
					else
					{
						builder.Add(new SkipTakeRestSymbol());
					}

					return true.Success();
				}

				var ((matched, value), (failed, exception)) = getExpression(state, ExpressionFlags.OmitComma);
				if (matched)
				{
					var skip = none<Expression>();
					var take = value.Some();
					if (first)
					{
						builder.Add(new SkipTakeInitSymbol(skip, take));
					}
					else
					{
						builder.Add(new SkipTakeSymbol(skip, take));
					}

					return true.Success();
				}
				else if (failed)
				{
					return failure<bool>(exception);
				}
				else
				{
					return "Expected expression".Failure<bool>();
				}
			}
			else
			{
				var skip = none<Expression>();
				var take = none<Expression>();
				var ((matched, value), (failed, exception)) = getExpression(state, ExpressionFlags.OmitColon);
				if (matched)
				{
					skip = value.Some();
					if (state.Scan("^ /(|s|) /';'", Color.Whitespace, Color.Structure).IsMatched)
					{
						if (state.Scan("^ /(|s|) /'*'", Color.Whitespace, Color.Structure).IsMatched)
						{
							if (first)
							{
								builder.Add(new SkipTakeRestInitSymbol());
							}
							else
							{
								builder.Add(new SkipTakeRestSymbol());
							}

							return true.Success();
						}

						((matched, value), (failed, exception)) = getExpression(state, ExpressionFlags.OmitComma);
						if (matched)
						{
							take = value.Some();
						}
						else if (failed)
						{
							return failure<bool>(exception);
						}
						else
						{
							return "Expected take expression".Failure<bool>();
						}
					}

					if (first)
					{
						builder.Add(new SkipTakeInitSymbol(skip, take));
					}
					else
					{
						builder.Add(new SkipTakeSymbol(skip, take));
					}

					return true.Success();
				}
				else if (failed)
				{
					return failure<bool>(exception);
				}
				else
				{
					return "Expected expression".Failure<bool>();
				}
			}
		}
	}
}