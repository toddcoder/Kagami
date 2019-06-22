using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class TraitParser : StatementParser
	{
		public override string Pattern => $"^ /'trait' /(|s+|) /({REGEX_CLASS})";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			var traitName = tokens[3].Text;
			state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class);

			var traitClass = new TraitClass(traitName);
			if (Module.Global.RegisterTrait(traitClass).If(out _, out var exception))
			{
				var traitItemsParser = new TraitItemsParser(traitClass);
				if (state.Advance().Out(out _, out var original))
				{
					while (state.More)
					{
						if (traitItemsParser.Scan(state).If(out _, out var anyException)) { }
						else if (anyException.If(out exception))
						{
							return failedMatch<Unit>(exception);
						}
						else
						{
							break;
						}
					}

					state.Regress();
					return Unit.Matched();
				}
				else
				{
					return original;
				}
			}
			else
			{
				return failedMatch<Unit>(exception);
			}
		}
	}
}