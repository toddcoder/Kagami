using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class CaseExpressionParser : SymbolParser
   {
      public CaseExpressionParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'|'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Structure);
         var result =
            from e1 in getExpression(state, ExpressionFlags.Comparisand)
            from equal in state.Scan("^ /(|s|) /'=' -(> '=')", Color.Whitespace, Color.Structure)
            from e2 in getExpression(state, builder.Flags)
            select (e1, e2);

         if (result.If(out var tuple, out var original))
         {
            Expressions = tuple;
            return Unit.Matched();
         }
         else
            return original.UnmatchedOnly<Unit>();
      }

      public (Expression, Expression) Expressions { get; set; }
   }
}