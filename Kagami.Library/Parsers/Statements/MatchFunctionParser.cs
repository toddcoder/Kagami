using System.Collections.Generic;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class MatchFunctionParser : StatementParser
	{
		public override string Pattern => $"^ /('override' /s+)? /'match' /(|s+|) (/({REGEX_CLASS_GETTING}) /'.')? " +
			$"/({REGEX_FUNCTION_NAME}) /'()'";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			var overriding = tokens[1].Text.StartsWith("override");
			var className = tokens[4].Text;
			var functionName = tokens[6].Text;
			var parameters = new Parameters(1);
			var parameterName = parameters[0].Name;
			state.Colorize(tokens, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure, Color.Invokable,
				Color.Structure);

			var list = new List<If>();

			if (state.Advance().Out(out _, out var original))
			{
				state.CreateReturnType();
				while (state.More)
				{
					var caseParser = new CaseParser(parameterName);
					state.SkipEndOfLine();
					if (caseParser.Scan(state).If(out _, out var anyException))
					{
						list.Add(caseParser.If);
					}
					else if (anyException.If(out var exception))
					{
						state.Regress();
						return failedMatch<Unit>(exception);
					}
					else
					{
						break;
					}
				}

				if (list.Count == 0)
				{
					state.Regress();
					state.RemoveReturnType();
					return notMatched<Unit>();
				}
				else
				{
					var stack = new Stack<If>();
					foreach (var ifStatement in list)
					{
						stack.Push(ifStatement);
					}

					var previousIf = stack.Pop();
					while (stack.Count > 0)
					{
						var current = stack.Pop();
						current.ElseIf = previousIf.Some();
						previousIf = current;
					}

					state.AddStatement(new MatchFunction(functionName, parameters, previousIf, overriding, className));
					state.Regress();
					state.RemoveReturnType();

					return Unit.Matched();
				}
			}
			else
			{
				return original;
			}
		}
	}
}