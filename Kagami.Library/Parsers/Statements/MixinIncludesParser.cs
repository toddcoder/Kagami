using System.Collections.Generic;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class MixinIncludesParser : StatementParser
{
   protected List<Mixin> mixins;

   public MixinIncludesParser(List<Mixin> mixins)
   {
      this.mixins = mixins;
   }

   public override string Pattern => "^ /'includes' /b";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Keyword);

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