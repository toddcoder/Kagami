using System.Text.RegularExpressions;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class TagParser : SymbolParser
{
   public TagParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@$"^(\s*)(:<{REGEX_FIELD}>)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var tag = tokens[2].Text.Drop(2).Drop(-1);
      state.Colorize(tokens, Color.Whitespace, Color.Message);
      builder.Add(new NewTagSymbol(tag));

      return unit;
   }
}