using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class OperatorsParser : SymbolParser
   {
      public OperatorsParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => $"^ /(|s|) /({REGEX_OPERATORS}1%2) -(>{REGEX_OPERATORS})";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.BeginTransaction();

         var whitespace = tokens[1].Text.IsNotEmpty();
         var source = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Operator);

         if (getOperator(state, source, builder.Flags, whitespace).If(out var symbol, out var original))
         {
            builder.Add(symbol);
            state.CommitTransaction();
            return Unit.Matched();
         }
         else
         {
            state.RollBackTransaction();
            return original.UnmatchedOnly<Unit>();
         }
      }
   }
}