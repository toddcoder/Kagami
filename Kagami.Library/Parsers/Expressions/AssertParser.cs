using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class AssertParser : SymbolParser
{
   public AssertParser(ExpressionBuilder builder) : base(builder) { }

   [GeneratedRegex(@"^(\s*)(assert)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var _result =
         from conditionValue in getExpression(state, builder.Flags | ExpressionFlags.OmitMaybe)
         from colon1 in state.Scan(@"^(\s*)(then)\b", Color.Whitespace, Color.Keyword)
         from valueValue in getExpression(state, builder.Flags | ExpressionFlags.OmitMaybe)
         from colon2 in state.Scan(@"^(\s*)(else)\b", Color.Whitespace, Color.Keyword)
         from failureExpressionValue in getExpression(state, builder.Flags | ExpressionFlags.OmitMaybe)
         select (conditionValue, valueValue, failureExpressionValue);

      if (_result is (true, var (condition, value, failureExpression)))
      {
         builder.Add(new AssertSymbol(condition, value, failureExpression));
         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }
}