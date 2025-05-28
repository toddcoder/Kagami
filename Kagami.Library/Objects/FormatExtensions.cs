using System.Text;
using Core.Strings;
using Kagami.Library.Parsers;

namespace Kagami.Library.Objects;

public static class FormatExtensions
{
   public static string FormatUsing<T>(this object obj, string format, Func<T, string> func)
   {
      if (obj is DateTime dateTime)
      {
         return dateTime.ToString(format);
      }
      else if (format.MatchOf(@"([cdefgnprxs])(-?\d+)?(?:\.(\d+))?") is (true, var matches))
      {
         var match = matches[0];
         var specifier = match.Groups[1].Value;
         var width = match.Groups[2].Value;
         var places = match.Groups[3].Value;
         var builder = new StringBuilder("{0");

         if (width.IsNotEmpty())
         {
            builder.Append($",{width}");
         }

         if (specifier.IsNotEmpty() && specifier != "s")
         {
            builder.Append($":{specifier}");
            if (places.IsNotEmpty())
            {
               builder.Append(places);
            }
         }

         builder.Append("}");
         return string.Format(builder.ToString(), obj);
      }
      else
      {
         return func((T)obj);
      }
   }
}