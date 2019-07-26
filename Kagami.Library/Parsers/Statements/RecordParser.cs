using System.Collections.Generic;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;

namespace Kagami.Library.Parsers.Statements
{
	public class RecordParser : StatementParser
	{
		public override string Pattern => $"^ /'record' /(|s+|) /({REGEX_CLASS}) /'('";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			var className = tokens[3].Text;
			state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);

			if (getParameters(state).Out(out var parameters, out var parametersOriginal))
			{
				var parentClassParser = new ParentClassParser();

				var parentClassName = "";
				var arguments = new Expression[0];
				if (parentClassParser.Scan(state).If(out _, out var anyException))
				{
					(parentClassName, _, arguments) = parentClassParser.Parent;
				}
				else if (anyException.If(out var exception))
				{
					return failedMatch<Unit>(exception);
				}

				Module.Global.ForwardReference(className);

				var builder = new ClassBuilder(className, parameters, parentClassName, arguments, false, new Block(), new List<Mixin>());
				if (builder.Register().Out(out _, out var registerOriginal))
				{
					var cls = new Class(builder);
					state.AddStatement(cls);

					return Unit.Matched();
				}
				else
				{
					return registerOriginal;
				}
			}
			else if (parametersOriginal.IsNotMatched)
			{
				return "parameters required".FailedMatch<Unit>();
			}
			else
			{
				return parametersOriginal.ExceptionAs<Unit>();
			}
		}
	}
}