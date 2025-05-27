using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class FoldOperatorParser : SymbolParser
{
   public FoldOperatorParser(ExpressionBuilder builder) : base(builder) { }

   //public override string Pattern => $"^ /(/s*) /(['<>'] ':') /({REGEX_OPERATORS}1%2) -(>{REGEX_OPERATORS})";

   [GeneratedRegex(@$"^(\s*)([<>]:)({REGEX_OPERATORS}{{1,2}})(?!{REGEX_OPERATORS})")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var prefix = tokens[2].Text;
      var source = tokens[3].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Operator);

      var _operator = getOperator(state, source, builder.Flags, true);
      if (_operator is (true, var symbol))
      {
         builder.Add(new FoldSymbol(prefix == "<:", symbol));
         return unit;
      }
      else
      {
         return _operator.Exception;
      }
   }
}