using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class NameOfParser : SymbolParser
{
   public NameOfParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex($@"^(\s*)(nameof)(\s+)({REGEX_INVOKABLE})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var name = tokens[4].Text;
      if (char.IsUpper(name[0]))
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Whitespace, Color.Class);
      }
      else
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Whitespace, Color.Identifier);
      }

      builder.Add(new NameOfSymbol(name));
      return unit;
   }
}