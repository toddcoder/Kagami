using System.Text;
using Core.Objects;
using Core.Strings;
using Kagami.Library.Parsers;

namespace Kagami.Library.Objects;

public static class FormatExtensions
{
   public static string FormatUsing<T>(this T obj, string format, Func<T, string> func) where T : notnull
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
      else if (format.MatchOf(@"([<=>])(\d+)") is (true, var matches2))
      {
         var match = matches2[0];
         var size = match.Groups[2].Value.Value().Int32();
         return match.Groups[1].Value switch
         {
            "<" => obj.ToString()?.LeftJustify(size) ?? "",
            "=" => obj.ToString()?.Center(size) ?? "",
            ">" => obj.ToString()?.RightJustify(size) ?? "",
            _ => obj.ToString() ?? ""
         };
      }
      else if (format.MatchOf(@"([\$\^&])") is (true, var matches3))
      {
         var match = matches3[0];
         return match.Groups[1].Value switch
         {
            "$" => obj.ToString()?.ToUpper() ?? "",
            "^" => obj.ToString()?.ToTitleCase() ?? "",
            "&" => obj.ToString()?.ToLower() ?? "",
            _ => obj.ToString() ?? ""
         };
      }
      else
      {
         return func(obj);
      }
   }
}