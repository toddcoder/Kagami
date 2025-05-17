using System;
using System.Text;
using Core.Matching;
using Core.Strings;

namespace Kagami.Library.Objects;

public static class FormatExtensions
{
   public static string FormatUsing<T>(this object obj, string format, Func<T, string> func)
   {
      if (obj is DateTime dateTime)
      {
         return dateTime.ToString(format);
      }
      else if (format.Matches("/['cdefgnprxs'] /('-'? /d+)? ('.' /(/d+))?") is (true, var result))
      {
         var (specifier, width, places) = result.Matches[0].Groups3();
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
         return string.Format(result.ToString(), obj);
      }
      else
      {
         return func((T)obj);
      }
   }
}