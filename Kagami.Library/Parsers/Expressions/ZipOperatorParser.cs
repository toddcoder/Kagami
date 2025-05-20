using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ZipOperatorParser : SymbolParser
{
   public ZipOperatorParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => $"^ /(|s|) /'[|' /({REGEX_OPERATORS}1%2) /'|]'";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var source = tokens[3].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Operator, Color.Operator);

      var _symbol = getOperator(state, source, builder.Flags, true);
      if (_symbol is (true, var symbol))
      {
         builder.Add(new ZipOperatorSymbol(symbol));
         return unit;
      }
      else
      {
         return _symbol.Exception;
      }
   }
}