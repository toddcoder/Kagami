using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class ImplicitOperatorParser : SymbolParser
{
   public ImplicitOperatorParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)\b(m|i|z|f|v|e)(>)(?!\.)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var type = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Operator);
      Symbol symbol = type switch
      {
         "z" => new ImplicitZip(),
         "f" => new ImplicitFold(),
         "v" => new ImplicitFoldVariable(),
         _ => new ImplicitSymbol(type)
      };
      builder.Add(symbol);

      return unit;
   }
}