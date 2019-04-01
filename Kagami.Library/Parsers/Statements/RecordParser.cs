using Kagami.Library.Classes;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class RecordParser : StatementParser
	{
		public override string Pattern => $"^ /'record' /(|s+|) /({REGEX_CLASS}) /'('";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			var className = tokens[3].Text;
			state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure);

			if (getParameters(state).Out(out var parameters, out var parametersOriginal))
			{
				var parentClassParser = new ParentClassParser();

				var parentClassName = "";
				var arguments = new Expression[0];
				if (parentClassParser.Scan(state).If(out _, out var mbException))
					(parentClassName, _, arguments) = parentClassParser.Parent;
				else if (mbException.If(out var exception))
					return failedMatch<Unit>(exception);

				Module.Global.ForwardReference(className);

				var builder = new ClassBuilder(className, parameters, parentClassName, arguments, false, new Block(),
					new Hash<string, TraitClass>());
				if (builder.Register().Out(out _, out var registerOriginal))
				{
					var cls = new Class(builder);
					state.AddStatement(cls);

					return Unit.Matched();
				}
				else
					return registerOriginal;
			}
			else if (parametersOriginal.IsNotMatched)
				return "parameters required".FailedMatch<Unit>();
			else
				return parametersOriginal.ExceptionAs<Unit>();
		}
	}
}