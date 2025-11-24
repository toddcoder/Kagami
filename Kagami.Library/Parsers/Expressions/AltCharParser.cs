using System.Text.RegularExpressions;
using Core.Monads;
using Core.Objects;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class AltCharParser : SymbolParser
{
   public AltCharParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\\)(\d{1,3})")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var codeString = tokens[3].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Char, Color.Char);

      var _code = codeString.Maybe().Byte();
      if (_code is (true, var code))
      {
         builder.Add(new CharSymbol((char)code));
         return unit;
      }
      else
      {
         return fail("Char code must be with 0-255");
      }
   }
}