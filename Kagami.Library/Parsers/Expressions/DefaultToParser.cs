using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class DefaultToParser : SymbolParser
{
   public DefaultToParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\|)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);
      var _expression = ParserFunctions.getExpression(state, builder.Flags);
      if (_expression is (true, var expression))
      {
         builder.Add(new DefaultToSymbol(expression));
         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}