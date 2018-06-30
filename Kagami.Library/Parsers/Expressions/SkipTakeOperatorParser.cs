using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SkipTakeOperatorParser : SymbolParser
   {
      public SkipTakeOperatorParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /'{'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         if (state.Source.Take(state.Index).IsMatch("/s+ $"))
            return notMatched<Unit>();
         else
         {
            state.Colorize(tokens, Color.Structure);

            if (getSkipTakeItems(state).If(out var arguments, out var original))
            {
               builder.Add(new SkipTakeOperatorSymbol(arguments));
               return Unit.Matched();
            }
            else
               return original.UnmatchedOnly<Unit>();
         }
      }
   }
}