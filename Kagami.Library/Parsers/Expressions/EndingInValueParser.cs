using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public abstract class EndingInValueParser : SymbolParser
{
   public EndingInValueParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public abstract Optional<Unit> Prefix(ParseState state, Token[] tokens);

   public abstract Optional<Unit> Suffix(ParseState state, Symbol value);

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      return Prefix(state, tokens).Map(_ => getValue(state, builder.Flags)).Map(s => Suffix(state, s));
   }
}