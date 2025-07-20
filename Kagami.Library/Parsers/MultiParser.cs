using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers;

public abstract class MultiParser : PatternlessParser
{
   public abstract IEnumerable<Parser> Parsers { get; }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens)
   {
      foreach (var parser in Parsers)
      {
         var _matched = parser.Scan(state);
         if (_matched)
         {
            return unit;
         }
         else if (_matched.Exception is (true, var exception))
         {
            return exception;
         }
      }

      return nil;
   }

   protected MultiParser() : base(false)
   {
   }
}