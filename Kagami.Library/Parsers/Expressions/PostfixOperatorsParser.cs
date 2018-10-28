using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class PostfixOperatorsParser : SymbolParser
   {
      public override string Pattern => "^ /(['?!']1%2) -(>['?!'])";

      public PostfixOperatorsParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var source = tokens[1].Text;
         state.Colorize(tokens, Color.Operator);

         switch (source)
         {
            case "?":
               builder.Add(new SomeSymbol());
               break;
            case "!":
	            builder.Add(new SuccessSymbol());
					break;
            default:
               return notMatched<Unit>();
         }

         return Unit.Matched();
      }
   }
}