﻿using System.Collections.Generic;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class MatchLambdaParser : SymbolParser
	{
		public override string Pattern => "^ /(|s|) /'|('";

		public MatchLambdaParser(ExpressionBuilder builder) : base(builder) { }

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Structure);

			var result =
				from c in getExpression(state, builder.Flags | ExpressionFlags.Comparisand)
				from scanned in state.Scan("^ /')' /(|s|) /('->' | '=>')", Color.Structure, Color.Whitespace, Color.Structure)
				from typeConstraint in parseTypeConstraint(state)
				from b in getLambdaBlock(scanned.Contains("->"), state, builder.Flags, typeConstraint)
				select (c, b);

			if (result.If(out var tuple))
			{
				var (comparisand, block) = tuple;

				var expressionBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
				expressionBuilder.Add(new FieldSymbol("__$0"));
				expressionBuilder.Add(new MatchSymbol());
				expressionBuilder.Add(comparisand);

				if (expressionBuilder.ToExpression().If(out var comparison, out var exception))
				{
					var list = new List<Statement> { new If(comparison, block) };
					var lambdaSymbol = new LambdaSymbol(1, new Block(list));
					builder.Add(lambdaSymbol);
					return Unit.Matched();
				}
				else
				{
					return failedMatch<Unit>(exception);
				}
			}

			return Unit.Matched();
		}
	}
}