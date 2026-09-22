using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class JunctionParser : SymbolParser
{
   public JunctionParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(&|\||\^|!)(:)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var type = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Operator);

      builder.Add(new JunctionSymbol2(type));
      return unit;
   }
}