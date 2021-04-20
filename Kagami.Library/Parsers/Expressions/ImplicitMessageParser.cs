using System.Linq;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class ImplicitMessageParser : SymbolParser
   {
      protected static string parameters(int count)
      {
         return $"({Enumerable.Range(0, count).Select(_ => "_").ToString(",")})";
      }

      public ImplicitMessageParser(ExpressionBuilder builder) : base(builder)
      {
      }

      public override string Pattern => $"^ /(|s|) /({REGEX_ITERATOR_FUNCTIONS}) /'^'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var message = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword);

         if (getValue(state, builder.Flags).ValueOrCast<Unit>(out var symbol, out var asUnit))
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
                  fieldName = "__$1";
                  parameterCount = 2;
                  break;
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
                  if (state.ImplicitState.IsNone)
                  {
                     var newMessage = message == "z" || message == "zip" || message == "*" ? "zip(_,_)" : "cross(_,_)";
                     state.ImplicitState = new ImplicitState(symbol, newMessage, 2, "__$0").Some();
                     builder.Add(new FieldSymbol("__$0"));
                     return Unit.Matched();
                  }
                  else if (state.ImplicitState.If(out var implicitState))
                  {
                     implicitState.Two = symbol.Some();
                     builder.Add(new FieldSymbol("__$1"));
                     return Unit.Matched();
                  }

                  break;
               case ".":
                  message = "seq";
                  break;
            }

            state.ImplicitState = new ImplicitState(symbol, message + parameters(parameterCount), parameterCount, fieldName).Some();
            builder.Add(new FieldSymbol(fieldName));

            return Unit.Matched();
         }
         else
         {
            return asUnit;
         }
      }
   }
}