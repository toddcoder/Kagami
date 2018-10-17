using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class StaticParser : StatementParser
	{
		ClassBuilder classBuilder;

		public StaticParser(ClassBuilder classBuilder) => this.classBuilder = classBuilder;

		public override string Pattern => $"^ /'object' /({REGEX_EOL})";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Keyword, Color.Whitespace);

			if (getBlock(state).If(out var block, out var original))
			{
				var className = classBuilder.UserClass.Name;
				var metaClassName = $"__$meta{className}";
				var metaClassBuilder = new ClassBuilder(metaClassName, Parameters.Empty, "", new Expression[0], block,
					new Hash<string, TraitClass>());
				if (metaClassBuilder.Register().If(out _, out var registerOriginal))
				{
					var classItemsParser = new ClassItemsParser(metaClassBuilder);
					while (state.More)
						if (classItemsParser.Scan(state).If(out _, out var isNotMatched, out var exception)) { }
						else if (isNotMatched)
							break;
						else
							return failedMatch<Unit>(exception);

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
	}
}