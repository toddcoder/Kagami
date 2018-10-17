using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class ModuleParser : StatementParser
	{
		public override string Pattern => $"^ /'module' /(|s+|) /({REGEX_CLASS}) /({REGEX_EOL})";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			var className = tokens[3].Text;
			state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.Whitespace);

			var parameters = Parameters.Empty;

			var parentClassName = "";
			var arguments = new Expression[0];
			Module.Global.ForwardReference(className);

			var builder = new ClassBuilder(className, parameters, parentClassName, arguments, new Block(),
				new Hash<string, TraitClass>());
			if (builder.Register().If(out _, out var registerOriginal))
			{
				var cls = new Class(builder);
				state.AddStatement(cls);

				if (getBlock(state).If(out var block, out var original))
				{
					var metaClassName = $"__$meta{className}";
					var metaClassBuilder = new ClassBuilder(metaClassName, Parameters.Empty, "", new Expression[0], block, new Hash<string, TraitClass>());
					if (metaClassBuilder.Register().If(out _, out registerOriginal))
					{
						var metaClass = new MetaClass(className, metaClassBuilder);
						state.AddStatement(metaClass);

						return Unit.Matched();
					}
					else
						return registerOriginal.Unmatched<Unit>();
				}
				else
					return original.Unmatched<Unit>();
			}
			else
				return registerOriginal.Unmatched<Unit>();
		}
	}
}