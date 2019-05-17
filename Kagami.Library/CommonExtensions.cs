using System.Linq;
using Kagami.Library.Objects;
using Core.Enumerables;
using Core.RegularExpressions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library
{
   public static class CommonExtensions
   {
      public static string get(this string name) => $"__${name}()";

      public static string unget(this string name) => name.Substitute("^ '__$' /(.*) $", "$1");

      public static string set(this string name) => $"__${name}=(_)";

	   public static Selector Selector(this string baseName, params string[] selectorItemSources)
      {
         if (selectorItemSources.Length == 0)
            return new Selector(baseName);
         else
         {
            var selectorItems = selectorItemSources.Select(parseSelectorItem).ToArray();
            var image = $"{baseName}({selectorItemSources.Join(",")})";

            return new Selector(baseName, selectorItems, image);
         }
      }

	   public static Selector Selector(this string baseName, int count)
	   {
		   return baseName.Selector(Enumerable.Range(0, count).Select(i => "_").ToArray());
	   }
   }
}