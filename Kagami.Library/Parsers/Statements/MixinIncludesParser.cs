using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class MixinIncludesParser : StatementParser
{
   protected List<Mixin> mixins;

   public MixinIncludesParser(List<Mixin> mixins)
   {
      this.mixins = mixins;
   }

   [GeneratedRegex(@"^(\s*)(:)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      while (state.More)
      {
         var parser = new MixinNameParser(mixins);
         var _scan = parser.Scan(state);
         if (_scan)
         {
         }
         else if (_scan.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      return unit;
   }
}