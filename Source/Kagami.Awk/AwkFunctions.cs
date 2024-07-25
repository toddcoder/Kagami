using System.Collections.Generic;
using Kagami.Library.Objects;
using Core.Matching;

namespace Kagami.Awk
{
   public static class AwkFunctions
   {
      public static string[] awkSplit(string input, Regex regex)
      {
         var fields = input.Unjoin(regex.Pattern); //input.Split(regex.Pattern, regex.IgnoreCase, regex.Multiline);
         var list = new List<string> { input };
         list.AddRange(fields);

         return list.ToArray();
      }

      public static string[] regexSplit(string input, Regex regex) =>
         input.Unjoin(regex.Pattern); //input.Split(regex.Pattern, regex.IgnoreCase, regex.Multiline);

      public static string[] awkInsert(string[] fields, int index, string item)
      {
         var list = new List<string>(fields);
         list.AddRange(fields);

         for (var i = list.Count; i < index; i++)
         {
            list.Add("");
         }

         list.Add(item);

         return list.ToArray();
      }
   }
}