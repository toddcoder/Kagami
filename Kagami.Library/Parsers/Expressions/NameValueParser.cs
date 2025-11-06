using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class NameValueParser : SymbolParser
{
   protected string name = "";

   public NameValueParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex($@"^(\s*)({REGEX_FIELD})(:)(\s+)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      name = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Label, Color.Operator, Color.Whitespace);

      var _expression = getExpression(state, builder.Flags | ExpressionFlags.OmitColon | ExpressionFlags.OmitComma);
      if (_expression is (true, var expression))
      {
         builder.Add(new NameValueSymbol(name, expression));
         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}