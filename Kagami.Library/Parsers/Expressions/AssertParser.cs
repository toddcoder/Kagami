using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class AssertParser : SymbolParser
{
   public AssertParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(assert)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);

      var flags = builder.Flags;
      flags[ExpressionFlags.OmitColon] = true;
      flags[ExpressionFlags.OmitMaybe] = true;
      var _result =
         from conditionValue in getExpression(state, flags)
         from colon1 in state.Scan(@"^(\s*)(:)", Color.Whitespace, Color.Operator)
         from valueValue in getExpression(state, flags)
         from colon2 in state.Scan(@"^(\s*)(:)", Color.Whitespace, Color.Operator)
         from failureExpressionValue in getExpression(state, flags)
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