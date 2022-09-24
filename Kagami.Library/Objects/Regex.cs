using System;
using System.Linq;
using System.Text;
using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public readonly struct Regex : IObject, ITextFinding
   {
      private static IObject getMatchOrText(RegexMatch match, bool textOnly) => textOnly ? match.Text : match;

      private readonly Core.Matching.Pattern pattern;
      private readonly bool ignoreCase;
      private readonly bool multiline;
      private readonly bool global;
      private readonly bool textOnly;
      private readonly Func<MatchResult, Func<string, Maybe<int>>> nameToIndex;

      public Regex(string pattern, bool ignoreCase, bool multiline, bool global, bool textOnly) : this()
      {
         this.pattern = pattern;
         this.pattern = this.pattern.WithIgnoreCase(ignoreCase);
         this.pattern = this.pattern.WithMultiline(multiline);
         this.ignoreCase = ignoreCase;
         this.multiline = multiline;
         this.global = global;
         this.textOnly = textOnly;

         nameToIndex = m => m.IndexFromName;
      }

      public string ClassName => "Regex";

      public string AsString => pattern.Regex;

      public string Image
      {
         get
         {
            var builder = new StringBuilder("/");
            builder.Append(pattern.Regex);
            if (ignoreCase || multiline || global)
            {
               builder.Append(";");
            }

            if (ignoreCase)
            {
               builder.Append("i");
            }

            if (multiline)
            {
               builder.Append("m");
            }

            if (global)
            {
               builder.Append("g");
            }

            if (textOnly)
            {
               builder.Append("t");
            }

            builder.Append("/");

            return builder.ToString();
         }
      }

      public int Hash => Image.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Regex regex && Image == regex.Image;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => pattern.Regex.Length > 0;

      private Maybe<MatchResult> isMatch(string input) => input.Matches(pattern);

      public IObject Matches(string input)
      {
         var self = this;
         var _result = isMatch(input);

         if (global)
         {
            if (_result)
            {
               return new Tuple(_result.Value.Select(m => new RegexMatch(m, self.nameToIndex(self.matcher)))
                  .Select(m => getMatchOrText(m, self.textOnly)).ToArray());
            }
            else
            {
               return Tuple.Empty;
            }
         }
         else if (isMatch(input))
         {
            return Some.Object(getMatchOrText(new RegexMatch(matcher.GetMatch(0), self.nameToIndex(self.matcher)), self.textOnly));
         }
         else
         {
            return None.NoneValue;
         }
      }

      public Boolean NotMatches(string input) => !isMatch(input);

      public String Replace(string input, string replacement)
      {
         if (global)
         {
            return input.Substitute(pattern, replacement, ignoreCase, multiline);
         }
         else
         {
            return input.Substitute(pattern, replacement, 1, ignoreCase, multiline);
         }
      }

      public Boolean IsMatch(string input) => matcher.IsMatch(input, pattern, ignoreCase, multiline);

      public IObject Find(string input, int startIndex, bool reverse)
      {
         if (input.MatchAll(pattern, ignoreCase, multiline).If(out var matches))
         {
            if (startIndex.Between(0).Until(matches.Length))
            {
               return Some.Object(Int.IntObject(matches[startIndex].Index));
            }
            else
            {
               return None.NoneValue;
            }
         }
         else
         {
            return None.NoneValue;
         }
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
            {
               return input;
            }
         }
         else
         {
            return input.Substitute(pattern, replacement, 1, ignoreCase, multiline);
         }
      }

      public String Replace(string input, Lambda lambda, bool reverse)
      {
         if (lambda.Invokable.Parameters.Length == 1)
         {
            return replace1(input, lambda, reverse);
         }
         else
         {
            return replace3(input, lambda, reverse);
         }
      }

      private String replace3(string input, Lambda lambda, bool reverse)
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
            {
               return input;
            }
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
            {
               return input;
            }
         }
      }

      private String replace1(string input, Lambda lambda, bool reverse)
      {
         if (reverse)
         {
            if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
            {
               var matchIndex = matcher.MatchCount - 1;
               var (text, _, _) = matcher.GetMatch(matchIndex);
               var replacement = lambda.Invoke((String)text);
               var result = text.Substitute(pattern, replacement.AsString, ignoreCase, multiline);
               matcher[matchIndex] = result;

               return matcher.ToString();
            }
            else
            {
               return input;
            }
         }
         else
         {
            if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
            {
               var (text, _, _) = matcher.GetMatch(0);
               var replacement = lambda.Invoke((String)text);
               var result = text.Substitute(pattern, replacement.AsString, ignoreCase, multiline);
               matcher[0] = result;

               return matcher.ToString();
            }
            else
            {
               return input;
            }
         }
      }

      public String ReplaceAll(string input, string replacement) => input.Substitute(pattern, replacement, ignoreCase, multiline);

      public String ReplaceAll(string input, Lambda lambda)
      {
         if (lambda.Invokable.Parameters.Length == 1)
         {
            return replaceAll1(input, lambda);
         }
         else
         {
            return replaceAll3(input, lambda);
         }
      }

      private String replaceAll3(string input, Lambda lambda)
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
         {
            return input;
         }
      }

      private String replaceAll1(string input, Lambda lambda)
      {
         if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
            {
               var (text, _, _) = matcher.GetMatch(i);
               var replacement = lambda.Invoke((String)text);
               matcher[i] = replacement.AsString;
            }

            return matcher.ToString();
         }
         else
         {
            return input;
         }
      }

      public Tuple Split(string input) => new(input.Split(pattern, ignoreCase, multiline).Select(String.StringObject).ToArray());

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
            {
               return Tuple.Tuple3(input, "", "");
            }
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
            {
               return Tuple.Tuple3(input, "", "");
            }
         }
      }

      public Int Count(string input) => matcher.IsMatch(input, pattern, ignoreCase, multiline) ? matcher.MatchCount : 0;

      public Int Count(string input, Lambda lambda)
      {
         if (lambda.Invokable.Parameters.Length == 1)
         {
            return count1(input, lambda);
         }
         else
         {
            return count3(input, lambda);
         }
      }

      private Int count3(string input, Lambda lambda)
      {
         if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
         {
            var count = 0;
            for (var i = 0; i < matcher.MatchCount; i++)
            {
               var (text, index, length) = matcher.GetMatch(i);
               if (lambda.Invoke((String)text, (Int)index, (Int)length).IsTrue)
               {
                  count++;
               }
            }

            return count;
         }
         else
         {
            return 0;
         }
      }

      private Int count1(string input, Lambda lambda)
      {
         if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
         {
            var count = 0;
            for (var i = 0; i < matcher.MatchCount; i++)
            {
               var (text, _, _) = matcher.GetMatch(i);
               if (lambda.Invoke((String)text).IsTrue)
               {
                  count++;
               }
            }

            return count;
         }
         else
         {
            return 0;
         }
      }

      public Regex Concatenate(IObject obj) => obj switch
      {
         Regex regex => new Regex(pattern + regex.pattern, ignoreCase, multiline, global, textOnly),
         String str => new Regex(pattern + str.Value, ignoreCase, multiline, global, textOnly),
         _ => new Regex(pattern + obj.AsString, ignoreCase, multiline, global, textOnly)
      };

      public Regex Concatenate(string otherPattern) => new(pattern + otherPattern, ignoreCase, multiline, global, textOnly);

      public IMatched<Matcher.Match> MatchOne(string input) => input.MatchOne(pattern, ignoreCase, multiline);

      public string Pattern => pattern;

      public bool IgnoreCase => ignoreCase;

      public bool Multiline => multiline;
   }
}