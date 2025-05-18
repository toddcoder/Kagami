using Core.Monads;

namespace Kagami.Library.Parsers.Expressions;

public abstract class SymbolParser : Parser
{
   protected ExpressionBuilder builder;

   protected SymbolParser(ExpressionBuilder builder) : base(false) => this.builder = builder;

   public abstract Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder);

   public override Optional<Unit> Parse(ParseState state, Token[] tokens) => Parse(state, tokens, builder);
}