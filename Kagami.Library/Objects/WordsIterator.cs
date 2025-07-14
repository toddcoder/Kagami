using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using RRegex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Objects;

public class WordsIterator(KString kString) : Iterator(kString)
{
   protected RRegex regex = new(@"\s+");
   protected string currentString = kString.Value;

   public override Maybe<IObject> Next()
   {
      if (currentString.IsEmpty())
      {
         return nil;
      }

      var match = regex.Match(currentString);
      if (match.Success)
      {
         var stringToReturn = currentString.Keep(match.Index);
         currentString = currentString.Drop(match.Index + match.Length);

         return KString.StringObject(stringToReturn).Some();
      }
      else
      {
         var remainingString = currentString;
         currentString = "";

         return KString.StringObject(remainingString).Some();
      }
   }
}