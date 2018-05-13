using System.Linq;
using System.Text;
using Standard.Types.Collections;
using Standard.Types.RegularExpressions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Arrays.ArrayFunctions;

namespace Kagami.Library.Objects
{
   public struct Regex : IObject
   {
      public static Tuple getTuple(Matcher.Match match)
      {
         return new Tuple(array(String.Object(match.Text), Int.Object(match.Index), Int.Object(match.Length), getTuple(match.Groups)));
      }

      public static IObject getTuple(Matcher.Group[] groups)
      {
         var objects = groups.Select(g =>
         {
            var innerArray = array(String.Object(g.Text), Int.Object(g.Index), Int.Object(g.Length));
            var innerTuple = new Tuple(innerArray);
            return (IObject)innerTuple;
         }).ToArray();

         return new Tuple(objects);
      }

      string pattern;
      bool ignoreCase;
      bool multiline;
      bool global;
      Matcher matcher;

      public Regex(string pattern, bool ignoreCase, bool multiline, bool global) : this()
      {
         this.pattern = pattern;
         this.ignoreCase = ignoreCase;
         this.multiline = multiline;
         this.global = global;

         matcher = new Matcher();
      }

      public string ClassName => "Regex";

      public string AsString => matcher.Pattern;

      public string Image
      {
         get
         {
            var builder = new StringBuilder("/");
            builder.Append(pattern);
            if (ignoreCase || multiline || global)
               builder.Append(";");
            if (ignoreCase)
               builder.Append("i");
            if (multiline)
               builder.Append("m");
            if (global)
               builder.Append("g");
            builder.Append("/");

            return builder.ToString();
         }
      }

      public int Hash => Image.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Regex regex && Image == regex.Image;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => pattern.Length > 0;

      bool isMatch(string input) => matcher.IsMatch(input, pattern, ignoreCase, multiline);

      public IObject Match(string input)
      {
         if (global)
            if (isMatch(input))
               return new Tuple(matcher.AllMatches.Select(m => (IObject)getTuple(m)).ToArray());
            else
               return Tuple.Empty;
         else if (isMatch(input))
            return Some.Object(getTuple(matcher.AllMatches[0]));
         else
            return Nil.NilValue;
      }

      public IObject MatchString(string input)
      {
         if (global)
            if (isMatch(input))
               return new Tuple(matcher.AllMatches.Select(m => String.Object(m.Text)).ToArray());
            else
               return Tuple.Empty;
         else if (isMatch(input))
            return Some.Object(String.Object(matcher.AllMatches[0].Text));
         else
            return Nil.NilValue;
      }

      public String Replace(string input, string replacement)
      {
         if (global)
            return input.Substitute(pattern, replacement, ignoreCase, multiline);
         else
            return input.Substitute(pattern, replacement, 1, ignoreCase, multiline);
      }

      public Boolean IsMatch(string input) => matcher.IsMatch(input, pattern, ignoreCase, multiline);

      public Tuple Split(string input)
      {
         return new Tuple(input.Split(pattern, ignoreCase, multiline).Select(String.Object).ToArray());
      }
   }
}