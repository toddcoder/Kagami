using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;
using Standard.Types.Strings;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class DateParser : SymbolParser
   {
      public DateParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /('d' [quote]) /(-[quote]*) /[quote]";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var text = tokens[3].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Date, Color.Date, Color.Date);

         if (text.DateTime().If(out var dateTime, out var exception))
         {
            builder.Add(new DateSymbol(dateTime));
            return Unit.Matched();
         }
         else
            return failedMatch<Unit>(exception);
      }
   }
}