using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class PlaceholderParser : SymbolParser
{
   public PlaceholderParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex($@"^(\s*)((?:use|var)\s*)?({REGEX_FIELD})\b(?!"")")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var mutable = tokens[2].Text.Trim();
      var placeholderName = tokens[3].Text;
      if (placeholderName is "false" or "true")
      {
         return nil;
      }

      if (placeholderName.StartsWith('`'))
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Identifier);
         builder.Add(new FieldSymbol(placeholderName));
         return unit;
      }

      var name = mutable switch
      {
         "use" => placeholderName,
         "var" => $"+{placeholderName}",
         _ => $"-{placeholderName}"
      };
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Identifier);
      builder.Add(new PlaceholderSymbol(name));

      return unit;
   }
}