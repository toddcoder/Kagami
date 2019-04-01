using Kagami.Library.Nodes.Symbols;
using Core.Monads;

namespace Kagami.Library.Parsers.Expressions
{
	public class ReductionParser : SymbolParser
   {
		public override string Pattern => "^ /(|s+|) /'['";

		public ReductionParser(ExpressionBuilder builder) : base(builder) { }

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.BeginTransaction();
			state.Colorize(tokens, Color.Whitespace, Color.Operator);

			var innerBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
			innerBuilder.Add(new FieldSymbol("__$0"));
			var operatorsParser = new OperatorsParser(innerBuilder);

			var result =
				from op in operatorsParser.Scan(state)
				from closing in state.Scan("^ /']'", Color.Operator)
				select op;

			if (result.Out(out _, out var original))
			{
				innerBuilder.Add(new FieldSymbol("__$1"));
				if (innerBuilder.ToExpression().If(out var expression, out var exception))
				{
					var lambda = new LambdaSymbol(2, expression);
					builder.Add(new SendBinaryMessageSymbol("foldr", Precedence.ChainedOperator));
					builder.Add(lambda);
					state.CommitTransaction();

					return Unit.Matched();
				}
				else
				{
					state.RollBackTransaction();
					return MonadFunctions.failedMatch<Unit>(exception);
				}
			}
			else
			{
				state.RollBackTransaction();
				return original.Unmatched<Unit>();
			}
		}
   }
}