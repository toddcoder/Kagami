using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class BindingParser : SymbolParser
{
   protected string name = "";

   public BindingParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@$"^(\s*)(?:(use|var)(\s+))?({REGEX_FIELD})(')(?!.')")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var mutable = tokens[2].Text;
      var placeholderName = tokens[4].Text;
      name = mutable switch
      {
         "use" => placeholderName,
         "var" => $"+{placeholderName}",
         _ => $"-{placeholderName}"
      };
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Operator);

      builder.Add(new BindingSymbol(name));

      return unit;
   }
}