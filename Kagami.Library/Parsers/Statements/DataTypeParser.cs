using System.Collections.Generic;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;
using DataType = Kagami.Library.Nodes.Statements.DataType;

namespace Kagami.Library.Parsers.Statements
{
	public class DataTypeParser : StatementParser
	{
		public override string Pattern => $"^ /'type' /(/s+) /({REGEX_CLASS}) {REGEX_ANTICIPATE_END}";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			var className = tokens[3].Text;
			state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class);

			var values = new Hash<string, (IObject[], IRangeItem)>();
			var dataComparisandNames = new List<string>();
			var dataTypeClass = new DataTypeClass(className);
			if (Module.Global.RegisterClass(dataTypeClass).If(out _, out var exception))
			{
				Module.Global.ForwardReference(className);
				IRangeItem ordinal = (Int)0;

				if (state.Advance().Out(out _, out var original))
				{
					while (state.More)
					{
						var parser = new DataComparisandParser(className, values, ordinal);
						if (parser.Scan(state).If(out _, out var anyException))
							if (dataTypeClass.RegisterDataComparisand(parser.Name, (IObject)parser.Ordinal).If(out _, out var regException))
							{
								dataComparisandNames.Add(parser.Name);
								ordinal = parser.Ordinal.Successor;
							}
							else
								return failedMatch<Unit>(regException);
						else if (anyException.If(out exception))
							return failedMatch<Unit>(exception);
						else
							break;
					}

					state.Regress();
				}
				else
					return original;

				state.AddStatement(new DataType(className, values.ToHash(i => i.Key, i =>
				{
					var (data, rangeItem) = i.Value;
					return (data, (IObject)rangeItem);
				})));

				return Unit.Matched();
			}
			else
				return failedMatch<Unit>(exception);
		}
	}
}