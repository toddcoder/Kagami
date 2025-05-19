using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ImplicitExpressionParser : EndingInExpressionParser
{
   protected string message;
   protected int parameterCount;
   protected string fieldName;

   public ImplicitExpressionParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => $"^ /(|s|) /({REGEX_ITERATOR_FUNCTIONS}) /'>'";

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      message = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword);

      parameterCount = 1;
      fieldName = "__$0"; //!?*%$
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
            message = "each(_)";
            break;
         case "fold":
         case "$":
            message = "foldl";
            goto case "foldl";
         case "foldl":
         case "reducel":
            message += "(_,_)";
            fieldName = "__$1";
            parameterCount = 2;
            break;
         case "accr":
            message = "foldr";
            goto case "foldr";
         case "foldr":
         case "reducer":
            message += "(_,_)";
            parameterCount = 2;
            break;
         case "while":
         case "takeWhile":
            message = "takeWhile(_)";
            break;
         case "until":
         case "takeUntil":
            message = "takeUntil(_)";
            break;
         case "skipWhile":
            message = "skipWhile(_)";
            break;
         case "skipUntil":
            message = "skipUntil(_)";
            break;
         case "zip":
         case "z":
         case "cross":
         case "x":
         case "*":
            parameterCount = 2;
            message = message is "z" or "zip" or "*" ? "zip(_,_,_)" : "cross(_,_,_)";
            break;
         case "seq":
         case ".":
            message = "seq(_)";
            break;
         default:
            message += "(_)";
            break;
      }

      state.BeginImplicitExpressionState();
      state.ImplicitExpressionState.FieldName1 = fieldName;
      if (parameterCount == 2)
      {
         state.ImplicitExpressionState.FieldName2 = fieldName == "__$0" ? "__$1" : "__$0";
      }

      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      var implicitExpressionState = state.ImplicitExpressionState;
      state.EndImplicitExpressionState();
      if (implicitExpressionState.Symbol1.If(out var symbol))
      {
         var implicitExpressionSymbol = new ImplicitExpressionSymbol(expression, message, parameterCount, symbol, implicitExpressionState.Symbol2);
         builder.Add(implicitExpressionSymbol);

         return unit;
      }
      else
      {
         return fail("Collection or iterable not expressed");
      }
   }
}