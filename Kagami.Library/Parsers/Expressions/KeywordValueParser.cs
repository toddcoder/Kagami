using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class KeywordValueParser : SymbolParser
{
   public KeywordValueParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(nil|true|false|unit|undef|unassigned)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var word = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      switch (word)
      {
         case "nil":
            builder.Add(new NilSymbol());
            break;
         case "true":
            builder.Add(new BooleanSymbol(true));
            break;
         case "false":
            builder.Add(new BooleanSymbol(false));
            break;
         case "unit":
            builder.Add(new UnitSymbol());
            break;
         case "undef":
            builder.Add(new UndefinedSymbol());
            break;
         default:
            return nil;
      }

      return unit;
   }
}