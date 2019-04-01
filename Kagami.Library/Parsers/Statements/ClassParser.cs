using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class ClassParser : StatementParser
	{
		public override string Pattern => $"^ /'class' /(|s+|) /({REGEX_CLASS}) /'('?";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			var className = tokens[3].Text;
			var hasParameters = tokens[4].Text == "(";
			state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure);

			Parameters parameters;

			if (hasParameters)
				if (getParameters(state).Out(out parameters, out var parametersOriginal)) { }
				else if (parametersOriginal.IsNotMatched)
					parameters = new Parameters(0);
				else
					return parametersOriginal.ExceptionAs<Unit>();
			else
				parameters = Parameters.Empty;

			state.SkipEndOfLine();

			state.Advance();
			var parentClassParser = new ParentClassParser();

			var parentClassName = "";
			var initialize = false;
			var arguments = new Expression[0];
			if (parentClassParser.Scan(state).If(out _, out var mbException))
				(parentClassName, initialize, arguments) = parentClassParser.Parent;
			else if (mbException.If(out var exception))
			{
				state.Regress();
				return failedMatch<Unit>(exception);
			}

			var traits = new Hash<string, TraitClass>();
			while (state.More)
			{
				var traitImplementsParser = new TraitImplementsParser(traits);
				if (traitImplementsParser.Scan(state).If(out _, out mbException)) { }
				else if (mbException.If(out var exception))
				{
					state.Regress();
					return failedMatch<Unit>(exception);
				}
				else
					break;
			}

			state.SkipEndOfLine();
			state.Regress();

			Module.Global.ForwardReference(className);

			state.SkipEndOfLine();
			if (getBlock(state).Out(out var block, out var original))
			{
				var builder = new ClassBuilder(className, parameters, parentClassName, arguments, initialize, block, traits);
				if (builder.Register().Out(out _, out var registerOriginal))
				{
					var cls = new Class(builder);
					if (testImplementation(builder.UserClass, traits).IfNot(out var exception))
						return failedMatch<Unit>(exception);
					else
						state.AddStatement(cls);

					var classItemsParser = new ClassItemsParser(builder);
					while (state.More)
						if (classItemsParser.Scan(state).If(out _, out mbException)) { }
						else if (mbException.If(out exception))
							return failedMatch<Unit>(exception);
						else
							break;

					return Unit.Matched();
				}
				else
					return registerOriginal.Unmatched<Unit>();
			}
			else
				return original.Unmatched<Unit>();
		}

		static IResult<Unit> testImplementation(UserClass userClass, Hash<string, TraitClass> traits)
		{
			foreach (var item in traits)
				if (item.Value.RegisterImplementor(userClass).IfNot(out var exception))
					return failure<Unit>(exception);

			return Unit.Success();
		}
	}
}