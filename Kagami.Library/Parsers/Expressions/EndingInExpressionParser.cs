using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public abstract class EndingInExpressionParser : SymbolParser
   {
      ExpressionFlags flags;

      public EndingInExpressionParser(ExpressionBuilder builder, ExpressionFlags flags = ExpressionFlags.Standard) : base(builder)
      {
         this.flags = flags;
      }

      public abstract IMatched<Unit> Prefix(ParseState state, Token[] tokens);

      public abstract IMatched<Unit> Suffix(ParseState state, Expression expression);

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder) =>
	      from prefix in Prefix(state, tokens)
	      from expression in getExpression(state, builder.Flags | flags)
	      from suffix in Suffix(state, expression)
	      select suffix;
   }
}