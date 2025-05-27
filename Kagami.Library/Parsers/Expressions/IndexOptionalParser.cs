using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class IndexOptionalParser : SymbolParser
{
   public IndexOptionalParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => "^ /'[?'";

   [GeneratedRegex(@"^(\[\?)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Structure);

      return getArguments(state, builder.Flags).Map(e =>
      {
         builder.Add(new SendMessageSymbol("[?]", e));
         return unit;
      });
   }
}