using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class AssertParser : SymbolParser
{
   public AssertParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(/s*) /('assert' | 'maybe') /b";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var isSuccess = tokens[2].Text == "assert";
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var _result =
         from conditionValue in getExpression(state, builder.Flags | ExpressionFlags.OmitColon)
         from colon1 in state.Scan("^ /(/s*) /':'", Color.Whitespace, Color.Structure)
         from valueValue in getExpression(state, builder.Flags | ExpressionFlags.OmitColon)
         select (conditionValue, valueValue);

      if (isSuccess)
      {
         if (_result is (true, var (condition, value)))
         {
            var _expression =
               from colon2 in state.Scan("^ /(/s*) /':'", Color.Whitespace, Color.Structure)
               from error in getExpression(state, builder.Flags | ExpressionFlags.OmitColon)
               select error;
            if (_expression is (true, var expression))
            {
               builder.Add(new AssertSymbol(condition, value, expression));
               return unit;
            }
            else
            {
               return _expression.Exception;
            }
         }
         else
         {
            return _result.Exception;
         }
      }
      else if (_result is (true, var (condition, value)))
      {
         builder.Add(new AssertSymbol(condition, value, nil));
         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }
}