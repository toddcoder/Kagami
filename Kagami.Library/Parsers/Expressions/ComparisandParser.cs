using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ComparisandParser : SymbolParser
{
   public ComparisandParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(|s|) /'|'";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);
      var _expression = getTerm(state, ExpressionFlags.Comparisand);
      if (_expression is (true, var expression))
      {
         builder.Add(expression);
         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}