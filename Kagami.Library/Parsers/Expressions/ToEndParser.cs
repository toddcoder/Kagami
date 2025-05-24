using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class ToEndParser : SymbolParser
{
   public override string Pattern => "^ /'..' (>[')]'])";

   [GeneratedRegex(@"^(\.\.)(?=[\)\]])", RegexOptions.Compiled)]
   public override partial Regex Regex();

   public ToEndParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Operator);
      builder.Add(new PushObjectSymbol(End.Value));

      return unit;
   }
}