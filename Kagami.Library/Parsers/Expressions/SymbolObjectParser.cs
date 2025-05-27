using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class SymbolObjectParser : SymbolParser
{
   public SymbolObjectParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => $"^ /(/s*) /'`' /({REGEX_FIELD}) /b";

   [GeneratedRegex($@"^(\s*)(`)({REGEX_FIELD})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var name = tokens[3].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Symbol, Color.Symbol);

      if (builder.Flags[ExpressionFlags.Comparisand])
      {
         builder.Add(new FieldSymbol(name));
      }
      else
      {
         builder.Add(new SymbolSymbol(name));
      }

      return unit;
   }
}