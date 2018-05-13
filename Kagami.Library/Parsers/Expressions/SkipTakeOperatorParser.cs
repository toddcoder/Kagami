using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SkipTakeOperatorParser : SymbolParser
   {
      public SkipTakeOperatorParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /'{'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Structure);

         if (getArguments(state, builder.Flags).If(out var arguments, out var original))
         {
            builder.Add(new SkipTakeOperatorSymbol(arguments));
            return Unit.Matched();
         }
         else
            return original.UnmatchedOnly<Unit>();
      }
   }
}