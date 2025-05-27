using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers;

public static class RegexExtensions
{
   public static Maybe<MatchCollection> MatchOf(this string input, Regex regex)
   {
      var collection = regex.Matches(input);
      return collection.Count > 0 ? collection : nil;
   }

   public static IEnumerable<Group> AllGroups(this Match match)
   {
      foreach (Group group in match.Groups)
      {
         yield return group;
      }
   }
}