using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class SetParser : SymbolParser
{
   public SetParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => "^ /(/s*) /'⎩' /(/s*)";

   [GeneratedRegex(@$"^(\s*)({REGEX_SET_LEFT})")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Collection);

      var _expression = getExpression(state, @$"^(\s*)({REGEX_SET_RIGHT})", builder.Flags & ~ExpressionFlags.OmitComma, Color.Whitespace, Color.Collection);
      if (_expression is (true, var expression))
      {
         builder.Add(new SetSymbol(expression));
         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}