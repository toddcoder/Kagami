using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class SelectorParser : SymbolParser
{
   [GeneratedRegex($@"^(\s*)(&)({REGEX_FUNCTION_NAME})(\([^\)]*\))?")]
   public override partial Regex Regex();

   public SelectorParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      Selector selector = tokens[3].Text + tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Selector, Color.Selector, Color.Selector);
      builder.Add(new SelectorSymbol(selector));

      return unit;
   }
}