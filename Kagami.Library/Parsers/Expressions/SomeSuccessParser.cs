using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class SomeSuccessParser : SymbolParser
{
   public SomeSuccessParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(?<!\s)([\?!])(?=\s|$)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var some = tokens[1].Text == "?";
      state.Colorize(tokens, Color.Operator);

      if (some)
      {
         builder.Add(new SomeSymbol());
      }
      else
      {
         builder.Add(new SuccessSymbol());
      }

      return unit;
   }
}