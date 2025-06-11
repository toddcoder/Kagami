using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class PostfixOperatorsParser : SymbolParser
{
   [GeneratedRegex(@"^([\?!&]{1,2})(?![\?!&])")]
   public override partial Regex Regex();

   public PostfixOperatorsParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var source = tokens[1].Text;
      state.Colorize(tokens, Color.Operator);

      switch (source)
      {
         case "?":
            builder.Add(new SomeSymbol());
            break;
         case "!":
            builder.Add(new SuccessSymbol());
            break;
         case "&":
            builder.Add(new SendMessageSymbol("value".get()));
            break;
         default:
            return nil;
      }

      return unit;
   }
}