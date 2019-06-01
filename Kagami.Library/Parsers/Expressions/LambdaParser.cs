using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.RegularExpressions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public abstract class LambdaParser : SymbolParser
	{
		protected LambdaParser(ExpressionBuilder builder) : base(builder) { }

		public abstract IMatched<Parameters> ParseParameters(ParseState state, Token[] tokens);

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.BeginTransaction();
			state.CreateReturnType();

			var result =
				from parameters in ParseParameters(state, tokens)
				from scanned in state.Scan("^ /(|s|) /'->'", Color.Whitespace, Color.Structure)
				from typeConstraint in parseTypeConstraint(state)
				from block in getLambdaBlock(!state.CurrentSource.IsMatch("^ (/r /n | /r | /n)"), state,
					builder.Flags & ~ExpressionFlags.Comparisand | ExpressionFlags.InLambda, typeConstraint)
				select new LambdaSymbol(parameters, block);
			if (result.Out(out var lambdaSymbol, out var original))
			{
				builder.Add(lambdaSymbol);
				state.RemoveReturnType();
				state.CommitTransaction();

				return Unit.Matched();
			}
			else
			{
				state.RollBackTransaction();
				state.RemoveReturnType();

				return original.Unmatched<Unit>();
			}
		}
	}
}