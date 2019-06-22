using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers
{
   public abstract class MultiParser : Parser
   {
      public abstract IEnumerable<Parser> Parsers { get; }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens)
      {
         foreach (var parser in Parsers)
         {
            var matched = parser.Scan(state);
            if (!matched.IsNotMatched)
            {
	            return matched;
            }
         }

         return notMatched<Unit>();
      }

      protected MultiParser() : base(false) { }
   }
}