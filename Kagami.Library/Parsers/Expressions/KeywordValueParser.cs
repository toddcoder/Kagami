using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class KeywordValueParser : SymbolParser
   {
      public override string Pattern => "^ /(|s|) /('nil' | 'true' | 'false' | 'del') /b";

      public KeywordValueParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var word = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Keyword);

         switch (word)
         {
            case "nil":
               builder.Add(new NilSymbol());
               break;
            case "true":
               builder.Add(new BooleanSymbol(true));
               break;
            case "false":
               builder.Add(new BooleanSymbol(false));
               break;
            case "del":
               builder.Add(new DelSymbol());
               break;
            default:
               return notMatched<Unit>();
         }

         return Unit.Matched();
      }
   }
}