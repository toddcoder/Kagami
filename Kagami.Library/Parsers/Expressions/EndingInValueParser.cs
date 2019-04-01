using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public abstract class EndingInValueParser : SymbolParser
   {
	   public EndingInValueParser(ExpressionBuilder builder) : base(builder) { }

      public abstract IMatched<Unit> Prefix(ParseState state, Token[] tokens);

      public abstract IMatched<Unit> Suffix(ParseState state, Symbol value);

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         return Prefix(state, tokens).Map(u => getValue(state, builder.Flags)).Map(s => Suffix(state, s));
      }
   }
}