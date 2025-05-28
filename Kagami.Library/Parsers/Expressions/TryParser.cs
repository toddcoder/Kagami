using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class TryParser : EndingInExpressionParser
{
   public TryParser(ExpressionBuilder builder, ExpressionFlags flags = ExpressionFlags.Standard) : base(builder, flags)
   {
   }

   //public override string Pattern => "^ /(/s*) /'try' /b";

   [GeneratedRegex(@"^(\s*)(try)\b", RegexOptions.Compiled)]
   public override partial Regex Regex();


   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      builder.Add(new TrySymbol(expression));
      return unit;
   }
}