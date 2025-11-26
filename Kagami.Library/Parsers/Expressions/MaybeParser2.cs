using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class MaybeParser2 : SymbolParser
{
   public MaybeParser2(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\?\?)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);

      var flags = builder.Flags | ExpressionFlags.OmitMaybe;
      var _expression = getExpression(state, flags);
      if (_expression is (true, var expression))
      {
         builder.Add(new MaybeSymbol2(expression));
         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}