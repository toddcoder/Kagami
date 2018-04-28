using Standard.Types.Enumerables;
using Standard.Types.RegularExpressions;

namespace Kagami.Library
{
   public static class CommonExtensions
   {
      public static string get(this string name) => $"__${name}";

      public static string unget(this string name) => name.Substitute("^ '__$' /(.*) $", "$1");

      public static string set(this string name) => $"__${name}=";

      public static string Function(this string baseName, params string[] segments)
      {
         if (segments.Length == 0)
            return baseName;
         else
            return $"{baseName}${segments.Listify("$")}";
      }
   }
}