using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class IsParser : SymbolParser
   {
      public IsParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s+|) /'??'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
	      state.Colorize(tokens, Color.Whitespace, Color.Operator);
	      var result =
		      from comparisand in getExpression(state, ExpressionFlags.Comparisand)
		      from scanned in state.Scan("^ /(/s*) /':'", Color.Whitespace, Color.Operator)
		      from expression in getExpression(state, builder.Flags)
		      select (comparisand, expression);
	      if (result.Out(out var tuple, out var original))
	      {
		      builder.Add(new IsSymbol(tuple.comparisand, tuple.expression));
		      return Unit.Matched();
	      }
	      else
		      return original.Unmatched<Unit>();
      }
   }
}