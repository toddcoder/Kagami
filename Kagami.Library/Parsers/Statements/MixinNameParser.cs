using System.Collections.Generic;
using Kagami.Library.Runtime;
using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class MixinNameParser : Parser
	{
		List<Mixin> mixins;

		public MixinNameParser(List<Mixin> mixins) : base(true) => this.mixins = mixins;

		public override string Pattern => $"^ /(/s*) /({REGEX_CLASS}) (/(/s*) /',')?";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens)
		{
			var mixinName = tokens[2].Text;
			var more = tokens[4].Text == ",";
			state.Colorize(tokens, Color.Whitespace, Color.Class, Color.Whitespace, Color.Structure);

			if (Module.Global.Mixin(mixinName).If(out var mixin))
			{
				if (mixins.FirstOrNone().IsNone)
				{
					mixins.Add(mixin);
				}

				More = more;
				return Unit.Matched();
			}
			else
			{
				return failedMatch<Unit>(mixinNotFound(mixinName));
			}
		}

		public bool More { get; set; }
	}
}