using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.RegexExtensions;

namespace Kagami.Library.Parsers;

public abstract class PatternedParser : Parser
{
   public PatternedParser(bool updateLastStatement) : base(updateLastStatement)
   {
   }

   public abstract Regex Regex();

   public override Optional<Unit> Scan(ParseState state)
   {
      var _matches = state.CurrentSource.Matches(Regex());

      if (_matches is (true, var matches))
      {
         var match = matches[0];
         var index = state.Index;
         var _parsed = Parse(state, GetTokens(state, match));
         if (_parsed && updateLastStatement)
         {
            state.UpdateStatement(index, match.Length);
         }
         if (_parsed.Exception)
         {
            state.SetExceptionIndex();
         }

         return _parsed;
      }
      else
      {
         return nil;
      }
   }
}