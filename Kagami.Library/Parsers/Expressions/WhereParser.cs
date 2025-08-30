using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using System.Text.RegularExpressions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class WhereParser : SymbolParser
{
   public WhereParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(where)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);

      var _result =
         from expressionValue in builder.ToExpression(true).Optional()
         from scanned in state.Scan(@"^(\s*)(\()", Color.Whitespace, Color.OpenParenthesis)
         from taggedExpressionsValue in getTaggedExpressions(state)
         select (expressionValue, taggedExpressionsValue);
      if (_result is (true, var (expression, taggedExpressions)))
      {
         builder.Add(new WhereSymbol(expression, taggedExpressions));
         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }
}