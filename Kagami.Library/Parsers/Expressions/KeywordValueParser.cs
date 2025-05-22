using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class KeywordValueParser : SymbolParser
{
   public override string Pattern => "^ /(/s*) /('none' | 'true' | 'false' | 'del' | 'unit' | 'null') /b";

   public KeywordValueParser(ExpressionBuilder builder) : base(builder) { }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var word = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      switch (word)
      {
         case "none":
            builder.Add(new NoneSymbol());
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
         case "unit":
            builder.Add(new UnitSymbol());
            break;
         case "null":
            builder.Add(new EmptyListSymbol());
            break;
         default:
            return nil;
      }

      return unit;
   }
}