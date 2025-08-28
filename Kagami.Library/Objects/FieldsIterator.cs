using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class FieldsIterator : Iterator
{
   protected string input;
   protected bool matched;
   protected MatchResult matchResult = MatchResult.Empty;
   protected int matchIndex;

   public FieldsIterator(KString kString) : base(kString)
   {
      input = kString.Value;
   }

   public override Maybe<IObject> Next()
   {
      if (!matched)
      {
         matched = true;
         var _result = input.Matches("/S+");
         if (_result is (true, var result))
         {
            matchResult = result;
         }
         else
         {
            return nil;
         }
      }

      return matchIndex < matchResult.MatchCount ? KString.StringObject(matchResult[matchIndex++]).Some() : nil;
   }
}