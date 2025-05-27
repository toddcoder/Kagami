using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class ConversionParser : EndingInValueParser
{
   protected string message = "";

   public ConversionParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => "^ /(/s*) /('int' | 'float' | 'byte' | 'long' | 'complex' | 'rational') /(/s+)";

   [GeneratedRegex(@"^(\s*)(int|float|byte|long|complex|rational)(\s+)")]
   public override partial Regex Regex();

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      message = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);

      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Symbol value)
   {
      builder.Add(new ConversionSymbol(message, value));
      return unit;
   }
}