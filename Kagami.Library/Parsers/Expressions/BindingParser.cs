using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class BindingParser : EndingInExpressionParser
{
   protected string name = "";

   public BindingParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => $"^ /(/s*) /('use' | 'var') /(/s+) /({REGEX_FIELD}) /'@'";

   [GeneratedRegex(@$"^(\s*)(use|var)(\s+)({REGEX_FIELD})(\s*)(@)")]
   public override partial Regex Regex();

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      var mutable = tokens[2].Text;
      var placeholderName = tokens[4].Text;
      name = mutable switch
      {
         "use" => placeholderName,
         "var" => $"+{placeholderName}",
         _ => $"-{placeholderName}"
      };
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Operator);

      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      builder.Add(new BindingSymbol(name, expression));
      return unit;
   }
}