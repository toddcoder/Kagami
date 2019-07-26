using System.Collections.Generic;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class FunctionParser : StatementParser
	{
		public override string Pattern => $"^ /('override' /s+)? /('func' | 'op' | 'def') /(/s+) (/({REGEX_CLASS_GETTING}) /'.')?" +
			$" /({REGEX_FUNCTION_NAME}) /'('?";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			var overriding = tokens[1].Text.StartsWith("override");
			var isOperator = tokens[2].Text == "op";
			var isMacro = tokens[2].Text == "def";

			var trait = false;
			var className = tokens[4].Text;
			if (className.IsEmpty() && TraitName.IsNotEmpty())
			{
				className = TraitName;
				trait = true;
			}

			var functionName = tokens[6].Text;
			var type = tokens[7].Text;

			if (isOperator && !Module.Global.RegisterOperator(functionName))
			{
				return $"Operator {functionName} already registered".FailedMatch<Unit>();
			}

			var needsParameters = type == "(";
			if (needsParameters)
			{
				state.Colorize(tokens, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure, Color.Invokable,
					Color.OpenParenthesis);
				if (functionName.IsMatch("^ /w+ '=' $"))
				{
					functionName = $"__${functionName.Drop(-1)}=";
				}
			}
			else
			{
				state.Colorize(tokens, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure, Color.Invokable);
				functionName = $"__${functionName}";
			}

			state.CreateYieldFlag();
			state.CreateReturnType();

			if (GetAnyParameters(needsParameters, state).Out(out var parameters, out var original))
			{
				if (state.CurrentSource.IsMatch("^ /s* '|'"))
				{
					return getMatchFunction(state, functionName, parameters, overriding, className);
				}
				else if (state.CurrentSource.StartsWith("("))
				{
					return getCurriedFunction(state, functionName, parameters, overriding, className, trait).Map(f =>
					{
						if (isMacro)
						{
							state.RegisterMacro(f);
						}
						else
						{
							state.AddStatement(f);
						}

						return Unit.Matched();
					});
				}
				else
				{
					return getAnyBlock(state).Map(block =>
					{
						var yielding = state.RemoveYieldFlag();
						state.RemoveReturnType();
						var function = new Function(functionName, parameters, block, yielding, overriding, className);
						if (isMacro)
						{
							state.RegisterMacro(function);
						}
						else
						{
							state.AddStatement(function);
						}

						return Unit.Matched();
					});
				}
			}
			else
			{
				return original.Unmatched<Unit>();
			}
		}

		public string TraitName { get; set; } = "";

		public static IMatched<Parameters> GetAnyParameters(bool needsParameters, ParseState state)
		{
			if (needsParameters)
			{
				return getParameters(state);
			}
			else
			{
				return Parameters.Empty.Matched();
			}
		}

		protected static IMatched<Function> getCurriedFunction(ParseState state, string functionName, Parameters firstParameters,
			bool overriding, string className, bool trait)
		{
			var parametersStack = new Stack<Parameters>();
			while (state.More)
			{
				var result =
					from prefix in state.Scan("^ /'('", Color.OpenParenthesis)
					from p in getParameters(state)
					select p;
				if (result.If(out var parameters, out var anyException))
				{
					parametersStack.Push(parameters);
				}
				else if (anyException.If(out var exception))
				{
					state.RemoveYieldFlag();
					state.RemoveReturnType();

					return failedMatch<Function>(exception);
				}
				else
				{
					break;
				}
			}

			if (getAnyBlock(state).Out(out var block, out var original))
			{
				var yielding = state.RemoveYieldFlag();
				state.RemoveReturnType();
				var lambdaSymbol = none<LambdaSymbol>();
				while (parametersStack.Count > 0)
				{
					var parameters = parametersStack.Pop();
					lambdaSymbol = lambdaSymbol.FlatMap(l => getLambda(parameters, l), () => new LambdaSymbol(parameters, block)).Some();
				}

				if (lambdaSymbol.If(out var ls))
				{
					return new Function(functionName, firstParameters, new Block(new Return(new Expression(ls), none<TypeConstraint>())),
						yielding, overriding, className).Matched();
				}
				else
				{
					return notMatched<Function>();
				}
			}
			else
			{
				return original.Unmatched<Function>();
			}
		}

		protected static LambdaSymbol getLambda(Parameters parameters, LambdaSymbol previousLambdaSymbol)
		{
			return new LambdaSymbol(parameters, new Block(new Return(new Expression(previousLambdaSymbol), none<TypeConstraint>())));
		}

		protected static IMatched<Unit> getMatchFunction(ParseState state, string functionName, Parameters parameters, bool overriding,
			string className)
		{
			var list = new List<If>();

			if (state.Advance().Out(out _, out var original))
			{
				state.CreateReturnType();
				while (state.More)
				{
					var caseParser = new CaseParser(parameters[0].Name);
					state.SkipEndOfLine();
					if (caseParser.Scan(state).If(out _, out var anyException))
					{
						caseParser.If.AddReturnIf();
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