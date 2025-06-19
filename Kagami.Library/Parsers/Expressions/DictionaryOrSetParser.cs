using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class DictionaryOrSetParser : SymbolParser
{
   public DictionaryOrSetParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\{)(\s*)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Collection, Color.Whitespace);

      var _expression = getExpression(state, @"^(\s*)(\})", ExpressionFlags.Standard, Color.Whitespace, Color.Collection);
      if (_expression is (true, var expression))
      {
         builder.Add(new DictionaryOrSetSymbol(expression));
         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}