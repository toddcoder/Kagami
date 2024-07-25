using System.Collections.Generic;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class InnerComprehensionParser : SymbolParser
   {
      protected List<(Symbol, Expression, IMaybe<Expression>, string)> comprehensions;

      public InnerComprehensionParser(ExpressionBuilder builder, List<(Symbol, Expression, IMaybe<Expression>, string)> comprehensions) :
         base(builder)
      {
         this.comprehensions = comprehensions;
      }

      public override string Pattern => "^ /(|s|) /'for' -(> ['^>']) /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword);
         if (getInnerComprehension(state).ValueOrCast<Unit>(out var tuple, out var asUnit))
         {
            var (comparisand, source, ifExp) = tuple;
            var image = $"for {comparisand} := {source}";
            comprehensions.Add((comparisand, source, ifExp, image));

            return Unit.Matched();
         }
         else
         {
            return asUnit;
         }
      }
   }
}