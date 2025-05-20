using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers;

public abstract class MultiParser : Parser
{
   public abstract IEnumerable<Parser> Parsers { get; }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens)
   {
      foreach (var parser in Parsers)
      {
         var _matched = parser.Scan(state);
         if (!_matched)
         {
            return _matched.Exception;
         }
      }

      return nil;
   }

   protected MultiParser() : base(false)
   {
   }
}