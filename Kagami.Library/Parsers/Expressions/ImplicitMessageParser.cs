using System.Linq;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ImplicitMessageParser : SymbolParser
{
   protected static string parameters(int count)
   {
      return $"({Enumerable.Range(0, count).Select(_ => "_").ToString(",")})";
   }

   public ImplicitMessageParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => $"^ /(/s*) /({REGEX_ITERATOR_FUNCTIONS}) /'^'";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var message = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword);

      var _symbol = getValue(state, builder.Flags);
      if (_symbol is (true, var symbol))
      {
         var parameterCount = 1;
         var fieldName = "__$0";
         message = message switch
         {
            "!" => "map",
            "?" => "if",
            ":" => "bind",
            _ => message
         };

         switch (message)
         {
            case "for":
            case "@":
               message = "each";
               break;
            case "fold":
            case "$":
               message = "foldl";
               goto case "foldl";
            case "foldl":
            case "reducel":
               fieldName = "__$1";
               parameterCount = 2;
               break;
            case "accr":
               message = "foldr";
               goto case "foldr";
            case "foldr":
            case "reducer":
               parameterCount = 2;
               break;
            case "while":
               message = "takeWhile";
               break;
            case "until":
               message = "takeUntil";
               break;
            case "z":
            case "zip":
            case "*":
            case "x":
            case "cross":
               if (!state.ImplicitState)
               {
                  var newMessage = message is "z" or "zip" or "*" ? "zip(_,_)" : "cross(_,_)";
                  state.ImplicitState = new ImplicitState(symbol, newMessage, 2, "__$0");
                  builder.Add(new FieldSymbol("__$0"));

                  return unit;
               }
               else if (state.ImplicitState is (true, var implicitState))
               {
                  implicitState.Two = symbol.Some();
                  builder.Add(new FieldSymbol("__$1"));

                  return unit;
               }

               break;
            case ".":
               message = "seq";
               break;
         }

         state.ImplicitState = new ImplicitState(symbol, message + parameters(parameterCount), parameterCount, fieldName);
         builder.Add(new FieldSymbol(fieldName));

         return unit;
      }
      else
      {
         return _symbol.Exception;
      }
   }
}