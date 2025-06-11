using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class PostfixInvokeParser : SymbolParser
{
   [GeneratedRegex(@"^(\()")]
   public override partial Regex Regex();

   public PostfixInvokeParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.OpenParenthesis);

      return getArguments(state, builder.Flags).Map(arguments =>
      {
         builder.Add(new PostfixInvokeSymbol(arguments));
         return unit;
      });
   }
}