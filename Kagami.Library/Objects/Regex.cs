using System.Text;
using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Regex : IObject, ITextFinding, IEquatable<Regex>
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
         if (_result is (true, var result))
         {
            return new KTuple(result.Select(m => new RegexMatch(m, self.nameToIndex(result), input.Keep(m.Index), input.Drop(m.Index + m.Length)))
               .Select(m => getMatchOrText(m, self.textOnly)).ToArray());
         }
         else
         {
            return KTuple.Empty;
         }
      }
      else if (isMatch(input) is (true, var result2))
      {
         var match = result2.GetMatch(0);
         var regexMatch = new RegexMatch(match, self.nameToIndex(result2), input.Keep(match.Index), input.Drop(match.Index + match.Length));
         return Some.Object(getMatchOrText(regexMatch, self.textOnly));
      }
      else
      {
         return None.NoneValue;
      }
   }

   public KBoolean NotMatches(string input) => !isMatch(input);

   private Core.Matching.Pattern getFixedPattern() => pattern.WithMultiline(multiline).WithIgnoreCase(ignoreCase);

   public KString Replace(string input, string replacement)
   {
      var fixedPattern = getFixedPattern();
      if (global)
      {
         return input.Substitute(fixedPattern, replacement);
      }
      else
      {
         return input.Substitute(pattern, replacement, 1);
      }
   }

   public KString Replace(string input, Lambda lambda)
   {
      var _result = input.Matches(getFixedPattern());
      if (_result is (true, var result))
      {
         var builder = new StringBuilder();
         var lastIndex = 0;

         foreach (var match in result)
         {
            if (match.Groups.Length == 1)
            {
               builder.Append(input.AsSpan(lastIndex, match.Index - lastIndex));
               var replacement = lambda.Invoke((KString)match.Text);
               builder.Append(replacement.AsString);
               lastIndex = match.Index + match.Length;
            }
            else
            {
               foreach (var group in match)
               {
                  builder.Append(input.AsSpan(lastIndex, group.Index - lastIndex));
                  var replacement = lambda.Invoke((KString)group.Text);
                  builder.Append(replacement.AsString);
                  lastIndex = group.Index + group.Length;
               }
            }
         }

         builder.Append(input.Drop(lastIndex));
         return new KString(builder.ToString());
      }
      else
      {
         return input;
      }
   }

   public KBoolean IsMatch(string input) => input.IsMatch(getFixedPattern());

   public IObject Find(string input, int startIndex, bool reverse)
   {
      if (input.Matches(getFixedPattern()) is (true, var result))
      {
         if (startIndex.Between(0).Until(result.Matches.Length))
         {
            return Some.Object(Int.IntObject(result.Matches[startIndex].Index));
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

   public KTuple FindAll(string input)
   {
      if (input.Matches(getFixedPattern()) is (true, var result))
      {
         return new KTuple([..result.Matches.Select(m => Int.IntObject(m.Index))]);
      }
      else
      {
         return KTuple.Empty;
      }
   }

   public KString Replace(string input, string replacement, bool reverse)
   {
      var fixedPattern = getFixedPattern();
      if (reverse)
      {
         if (input.Matches(fixedPattern) is (true, var result))
         {
            var matchIndex = result.Matches.Length - 1;
            var match = result.Matches[matchIndex];
            var resultText = match.Text.Substitute(pattern, replacement);
            result.Matches[matchIndex].Text = resultText;

            return result.Text;
         }
         else
         {
            return input;
         }
      }
      else
      {
         return input.Substitute(fixedPattern, replacement, 1);
      }
   }

   public KString Replace(string input, Lambda lambda, bool reverse)
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

   private KString replace3(string input, Lambda lambda, bool reverse)
   {
      var fixedPattern = getFixedPattern();
      if (reverse)
      {
         if (input.Matches(fixedPattern) is (true, var result))
         {
            var matchIndex = result.MatchCount - 1;
            var (text, index, length) = result.GetMatch(matchIndex);
            var replacement = lambda.Invoke((KString)text, (Int)index, (Int)length);
            var substitute = text.Substitute(fixedPattern, replacement.AsString);
            result[matchIndex] = substitute;

            return result.Text;
         }
         else
         {
            return input;
         }
      }
      else
      {
         if (input.Matches(fixedPattern) is (true, var result))
         {
            var (text, index, length) = result.GetMatch(0);
            var replacement = lambda.Invoke((KString)text, (Int)index, (Int)length);
            var substitute = text.Substitute(fixedPattern, replacement.AsString);
            result[0] = substitute;

            return result.Text;
         }
         else
         {
            return input;
         }
      }
   }

   private KString replace1(string input, Lambda lambda, bool reverse)
   {
      var fixedPattern = getFixedPattern();
      if (reverse)
      {
         if (input.Matches(fixedPattern) is (true, var result))
         {
            var matchIndex = result.MatchCount - 1;
            var (text, _, _) = result.GetMatch(matchIndex);
            var replacement = lambda.Invoke((KString)text);
            var substitute = text.Substitute(fixedPattern, replacement.AsString);
            result[matchIndex] = substitute;

            return result.ToString();
         }
         else
         {
            return input;
         }
      }
      else
      {
         if (input.Matches(fixedPattern) is (true, var result))
         {
            var (text, _, _) = result.GetMatch(0);
            var replacement = lambda.Invoke((KString)text);
            var substitute = text.Substitute(fixedPattern, replacement.AsString);
            result[0] = substitute;

            return result.Text;
         }
         else
         {
            return input;
         }
      }
   }

   public KString ReplaceAll(string input, string replacement) => input.Substitute(getFixedPattern(), replacement);

   public KString ReplaceAll(string input, Lambda lambda)
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

   private KString replaceAll3(string input, Lambda lambda)
   {
      var fixedPattern = getFixedPattern();
      if (input.Matches(fixedPattern) is (true, var result))
      {
         for (var i = 0; i < result.MatchCount; i++)
         {
            var (text, index, length) = result.GetMatch(i);
            var replacement = lambda.Invoke((KString)text, (Int)index, (Int)length);
            result[i] = replacement.AsString;
         }

         return result.Text;
      }
      else
      {
         return input;
      }
   }

   private KString replaceAll1(string input, Lambda lambda)
   {
      var fixedPattern = getFixedPattern();
      if (input.Matches(fixedPattern) is (true, var result))
      {
         for (var i = 0; i < result.MatchCount; i++)
         {
            var (text, _, _) = result.GetMatch(i);
            var replacement = lambda.Invoke((KString)text);
            result[i] = replacement.AsString;
         }

         return result.Text;
      }
      else
      {
         return input;
      }
   }

   public KTuple Split(string input) => new(input.Unjoin(getFixedPattern()).Select(KString.StringObject).ToArray());

   public KTuple Partition(string input, bool reverse)
   {
      var fixedPattern = getFixedPattern();
      if (reverse)
      {
         if (input.Matches(fixedPattern) is (true, var result))
         {
            var (delimiter, index, length) = result.GetMatch(result.MatchCount - 1);
            var left = input.Keep(index);
            var right = input.Drop(index + length);

            return KTuple.Tuple3(left, delimiter, right);
         }
         else
         {
            return KTuple.Tuple3(input, "", "");
         }
      }
      else
      {
         if (input.Matches(fixedPattern) is (true, var result))
         {
            var (delimiter, index, length) = result.GetMatch(0);
            var left = input.Keep(index);
            var right = input.Drop(index + length);

            return KTuple.Tuple3(left, delimiter, right);
         }
         else
         {
            return KTuple.Tuple3(input, "", "");
         }
      }
   }

   public Int Count(string input) => input.Matches(getFixedPattern()).Map(r => r.MatchCount) | 0;

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
      var fixedPattern = getFixedPattern();
      if (input.Matches(fixedPattern) is (true, var result))
      {
         var count = 0;
         for (var i = 0; i < result.MatchCount; i++)
         {
            var (text, index, length) = result.GetMatch(i);
            if (lambda.Invoke((KString)text, (Int)index, (Int)length).IsTrue)
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
      var fixedPattern = getFixedPattern();
      if (input.Matches(fixedPattern) is (true, var result))
      {
         var count = 0;
         for (var i = 0; i < result.MatchCount; i++)
         {
            var (text, _, _) = result.GetMatch(i);
            if (lambda.Invoke((KString)text).IsTrue)
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
      Regex regex => new Regex(pattern.Regex + regex.pattern.Regex, ignoreCase, multiline, global, textOnly),
      KString str => new Regex(pattern.Regex + str.Value, ignoreCase, multiline, global, textOnly),
      _ => new Regex(pattern.Regex + obj.AsString, ignoreCase, multiline, global, textOnly)
   };

   public Regex Concatenate(string otherPattern) => new(pattern.Regex + otherPattern, ignoreCase, multiline, global, textOnly);

   public Optional<Match> MatchOne(string input) => getFixedPattern().MatchedBy(input).Map(r => r.Matches[0]);

   public string Pattern => pattern.Regex;

   public bool IgnoreCase => ignoreCase;

   public bool Multiline => multiline;

   public bool Equals(Regex other) => pattern.Equals(other.pattern) && ignoreCase == other.ignoreCase && multiline == other.multiline &&
      global == other.global && textOnly == other.textOnly && nameToIndex.Equals(other.nameToIndex);

   public override bool Equals(object? obj) => obj is Regex other && Equals(other);

   public override int GetHashCode() => HashCode.Combine(pattern, ignoreCase, multiline, global, textOnly, nameToIndex);

   public static bool operator ==(Regex left, Regex right) => left.Equals(right);

   public static bool operator !=(Regex left, Regex right) => !left.Equals(right);
}