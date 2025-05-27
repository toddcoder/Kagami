using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class AnyParser : SymbolParser
{
   public AnyParser(ExpressionBuilder builder) : base(builder) { }

   //public override string Pattern => "^ /(/s*) /'_' /b";

   [GeneratedRegex(@"^(\s*)(_)\b")]
   public override partial Regex Regex();


   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Identifier);
      builder.Add(new AnySymbol());

      return unit;
   }
}