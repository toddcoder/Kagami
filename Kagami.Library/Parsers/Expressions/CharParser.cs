using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class CharParser : SymbolParser
{
   public CharParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(|s|) /(\"'\" ('\\' ['xu'] ['a-f0-9']1%6 | '\\'? .) \"'\")";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Char);
      var source = tokens[2].Text.Drop(1).Drop(-1);

      switch (source.Length)
      {
         case 1:
            builder.Add(new CharSymbol(source[0]));
            break;
         case 2 when source.StartsWith("\\"):
         {
            var _ch = fromBackslash(source[1]);
            if (_ch is (true, var ch))
            {
               builder.Add(new CharSymbol(ch));
            }
            else
            {
               return _ch.Exception;
            }

            break;
         }
         default:
         {
            var _ch = fromHex(source.Drop(2));
            if (_ch is (true, var ch))
            {
               builder.Add(new CharSymbol(ch));
            }
            else
            {
               return _ch.Exception;
            }

            break;
         }
      }

      return unit;
   }
}