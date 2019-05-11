using System.Linq;
using System.Text;
using Core.Collections;
using Core.Monads;
using Core.Numbers;
using Core.RegularExpressions;
using Core.Strings;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct Regex : IObject, ITextFinding
   {
      string pattern;
      bool ignoreCase;
      bool multiline;
      bool global;
      bool textOnly;
      Matcher matcher;

      public Regex(string pattern, bool ignoreCase, bool multiline, bool global, bool textOnly) : this()
      {
         this.pattern = pattern;
         this.ignoreCase = ignoreCase;
         this.multiline = multiline;
         this.global = global;
         this.textOnly = textOnly;

         matcher = new Matcher();
      }

      public string ClassName => "Regex";

      public string AsString => matcher.Pattern;

      public string Image
      {
         get
         {
            var builder = new StringBuilder("/");
            matcher.IsMatch("", pattern);
            builder.Append(matcher.Pattern);
            if (ignoreCase || multiline || global)
               builder.Append(";");
            if (ignoreCase)
               builder.Append("i");
            if (multiline)
               builder.Append("m");
            if (global)
               builder.Append("g");
            if (textOnly)
               builder.Append("t");
            builder.Append("/");

            return builder.ToString();
         }
      }

      public int Hash => Image.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Regex regex && Image == regex.Image;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => pattern.Length > 0;

      bool isMatch(string input) => matcher.IsMatch(input, pattern, ignoreCase, multiline);

      public IObject Matches(string input)
      {
         if (global)
            if (isMatch(input))
               return new Tuple(matcher.Select(m => (IObject)new RegexMatch(m)).ToArray());
            else
               return Tuple.Empty;
         else if (isMatch(input))
            return Some.Object(new RegexMatch(matcher.GetMatch(0)));
         else
            return None.NoneValue;
      }

	   public Boolean NotMatches(string input) => !isMatch(input);

	   public IObject MatchString(string input)
      {
         if (global)
            if (isMatch(input))
               return new Tuple(matcher.Select(m => String.StringObject(m.Text)).ToArray());
            else
               return Tuple.Empty;
         else if (isMatch(input))
            return Some.Object(String.StringObject(matcher.GetMatch(0).Text));
         else
            return None.NoneValue;
      }

      public String Replace(string input, string replacement)
      {
         if (global)
            return input.Substitute(pattern, replacement, ignoreCase, multiline);
         else
            return input.Substitute(pattern, replacement, 1, ignoreCase, multiline);
      }

      public Boolean IsMatch(string input) => matcher.IsMatch(input, pattern, ignoreCase, multiline);

      public IObject Find(string input, int startIndex, bool reverse)
      {
         if (input.MatchAll(pattern, ignoreCase, multiline).If(out var matches))
            if (startIndex.Between(0).Until(matches.Length))
               return Some.Object(Int.IntObject(matches[startIndex].Index));
            else
               return None.NoneValue;
         else
            return None.NoneValue;
      }

      public Tuple FindAll(string input)
      {
         return input.MatchAll(pattern, ignoreCase, multiline)
            .FlatMap(matches => new Tuple(matches.Select(m => Int.IntObject(m.Index)).ToArray()), () => Tuple.Empty);
      }

      public String Replace(string input, string replacement, bool reverse)
      {
         if (reverse)
         {
            if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
            {
               var matchIndex = matcher.MatchCount - 1;
               var match = matcher.GetMatch(matchIndex);
               var result = match.Text.Substitute(pattern, replacement, ignoreCase, multiline);
               matcher[matchIndex] = result;

               return matcher.ToString();
            }
            else
               return input;
         }
         else
            return input.Substitute(pattern, replacement, 1, ignoreCase, multiline);
      }

      public String Replace(string input, Lambda lambda, bool reverse)
      {
         if (reverse)
         {
            if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
            {
               var matchIndex = matcher.MatchCount - 1;
               var (text, index, length) = matcher.GetMatch(matchIndex);
               var replacement = lambda.Invoke((String)text, (Int)index, (Int)length);
               var result = text.Substitute(pattern, replacement.AsString, ignoreCase, multiline);
               matcher[matchIndex] = result;

               return matcher.ToString();
            }
            else
               return input;
         }
         else
         {
            if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
            {
               var (text, index, length) = matcher.GetMatch(0);
               var replacement = lambda.Invoke((String)text, (Int)index, (Int)length);
               var result = text.Substitute(pattern, replacement.AsString, ignoreCase, multiline);
               matcher[0] = result;

               return matcher.ToString();
            }
            else
               return input;
         }
      }

      public String ReplaceAll(string input, string replacement) => input.Substitute(pattern, replacement, ignoreCase, multiline);

      public String ReplaceAll(string input, Lambda lambda)
      {
         if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
            {
               var (text, index, length) = matcher.GetMatch(i);
               var replacement = lambda.Invoke((String)text, (Int)index, (Int)length);
               matcher[i] = replacement.AsString;
            }

            return matcher.ToString();
         }
         else
            return input;
      }

      public Tuple Split(string input)
      {
         return new Tuple(input.Split(pattern, ignoreCase, multiline).Select(String.StringObject).ToArray());
      }

      public Tuple Partition(string input, bool reverse)
      {
         if (reverse)
         {
            if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
            {
               var (delimiter, index, length) = matcher.GetMatch(matcher.MatchCount - 1);
               var left = input.Keep(index);
               var right = input.Drop(index + length);

               return Tuple.Tuple3(left, delimiter, right);
            }
            else
               return Tuple.Tuple3(input, "", "");
         }
         else
         {
            if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
            {
               var (delimiter, index, length) = matcher.GetMatch(0);
               var left = input.Keep(index);
               var right = input.Drop(index + length);

               return Tuple.Tuple3(left, delimiter, right);
            }
            else
               return Tuple.Tuple3(input, "", "");
         }
      }

      public Int Count(string input) => matcher.IsMatch(input, pattern, ignoreCase, multiline) ? matcher.MatchCount : 0;

      public Int Count(string input, Lambda lambda)
      {
         if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
         {
            var count = 0;
            for (var i = 0; i < matcher.MatchCount; i++)
            {
	            var (text, index, length) = matcher.GetMatch(i);
	            if (lambda.Invoke((String)text, (Int)index, (Int)length).IsTrue)
                  count++;
            }

            return count;
         }
         else
            return 0;
      }

      public Regex Concatenate(IObject obj)
      {
         switch (obj)
         {
            case Regex regex:
               return new Regex(pattern + regex.pattern, ignoreCase, multiline, global, textOnly);
            case String str:
               return new Regex(pattern + str.Value, ignoreCase, multiline, global, textOnly);
            default:
               return new Regex(pattern + obj.AsString, ignoreCase, multiline, global, textOnly);
         }
      }

      public Regex Concatenate(string otherPattern) => new Regex(pattern + otherPattern, ignoreCase, multiline, global, textOnly);

      public IMatched<Matcher.Match> MatchOne(string input) => input.MatchOne(pattern, ignoreCase, multiline);

      public string Pattern => pattern;

      public bool IgnoreCase => ignoreCase;

      public bool Multiline => multiline;
   }
}