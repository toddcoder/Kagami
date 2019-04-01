using Kagami.Library.Classes;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Kagami.Library.Parsers.Statements.FunctionParser;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class SignatureParser : StatementParser
	{
		TraitClass traitClass;

		public SignatureParser(TraitClass traitClass) => this.traitClass = traitClass;

		public override string Pattern => $"^ /'abstract' /(/s+) /({REGEX_FUNCTION_NAME}) /'('?";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			var functionName = tokens[3].Text;
			var needsParameters = tokens[4].Text == "(";
			state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Invokable, Color.Structure);

			if (needsParameters)
			{
				if (functionName.IsMatch("^ /w+ '=' $"))
					functionName = "__$" + functionName.Drop(-1).set();
			}
			else
				functionName = "__$" + functionName;

			if (GetAnyParameters(needsParameters, state).Out(out var parameters, out var original))
			{
				var selector = parameters.Selector(functionName);
				if (traitClass.RegisterSignature(selector).If(out _, out var exception))
					return Unit.Matched();
				else
					return failedMatch<Unit>(exception);
			}
			else
				return original.Unmatched<Unit>();
		}
	}
}