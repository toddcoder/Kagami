using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using System.Text;
using Kagami.Library.Runtime;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Regex : IObject, ITextFinding, IEquatable<Regex>, IAccepting
{
   private static IObject getMatchOrText(RegexMatch match, bool textOnly) => textOnly ? match.Text : match;

   private static Hash<PatternWithOptions, Core.Matching.Pattern> cachedPattern = [];

   private Core.Matching.Pattern getPattern(string originalPattern)
   {
      var patternWithOptions = new PatternWithOptions(originalPattern, ignoreCase, multiline, global, textOnly);
      if (cachedPattern.Maybe[patternWithOptions] is (true, var foundPattern))
      {
         return foundPattern;
      }

      Core.Matching.Pattern pattern;
      if (originalPattern.Matches("-(<'\\') /('$') /(['A-Za-z_']['A-Za-z_0-9']*)") is (true, var results))
      {
         var builder = new StringBuilder();
         var lastIndex = 0;
         foreach (var match in results)
         {
            var fieldName = match.SecondGroup;
            var _field = Machine.Current.Value.Find(fieldName, true);
            if (_field is (true, var field))
            {
               var fieldValue = field.Value;
               var fieldString = fieldValue is Regex regex ? regex.originalPattern : fieldValue.AsString;
               builder.Append(originalPattern.AsSpan(lastIndex, match.Index - lastIndex));
               builder.Append(fieldString);
               lastIndex = match.Index + match.Length;
            }
         }

         builder.Append(originalPattern.AsSpan(lastIndex));
         pattern = builder.ToString().Replace(@"\$", "$");
      }
      else
      {
         pattern = originalPattern.Replace(@"\$", "$");
      }

      pattern = pattern.WithIgnoreCase(ignoreCase).WithMultiline(multiline);
      cachedPattern[patternWithOptions] = pattern;

      return pattern;
   }

   private readonly string originalPattern;
   private readonly bool ignoreCase;
   private readonly bool multiline;
   private readonly bool global;
   private readonly bool textOnly;
   private readonly Func<MatchResult, Func<string, Maybe<int>>> nameToIndex;
   private readonly Func<MatchResult, Func<int, Maybe<string>>> indexToName;

   public Regex(string pattern, bool ignoreCase, bool multiline, bool global, bool textOnly) : this()
   {
      originalPattern = pattern;
      this.ignoreCase = ignoreCase;
      this.multiline = multiline;
      this.global = global;
      this.textOnly = textOnly;

      nameToIndex = m => m.IndexFromName;
      indexToName = m => m.NameFromIndex;
   }

   public Regex(Core.Matching.Pattern pattern, bool global, bool textOnly)
   {
      originalPattern = pattern.ToString() ?? "";
      ignoreCase = pattern.IgnoreCase;
      multiline = pattern.Multiline;
      this.global = global;
      this.textOnly = textOnly;

      nameToIndex = m => m.IndexFromName;
      indexToName = m => m.NameFromIndex;
   }

   public string ClassName => "Regex";

   public string AsString => getPattern(originalPattern).Regex;

   public string Image
   {
      get
      {
         var builder = new StringBuilder("x\"");
         builder.Append(getPattern(originalPattern).Regex);
         if (ignoreCase || multiline || global)
         {
            builder.Append(';');
         }

         if (ignoreCase)
         {
            builder.Append('i');
         }

         if (multiline)
         {
            builder.Append('m');
         }

         if (global)
         {
            builder.Append('g');
         }

         if (textOnly)
         {
            builder.Append('t');
         }

         if (textOnly)
         {
            builder.Append('t');
         }

         builder.Append('"');

         return builder.ToString();
      }
   }

   public int Hash => Image.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Regex regex && Image == regex.Image;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => getPattern(originalPattern).Regex.Length > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public Core.Matching.Pattern CorePattern => getPattern(originalPattern);

   private Maybe<MatchResult> isMatch(string input) => input.Matches(getPattern(originalPattern));

   public IObject MatchesIndex(string input)
   {
      var _result = isMatch(input);
      if (_result is (true, var result))
      {
         return Int.IntObject(result.Index);
      }
      else
      {
         return KNil.NilValue;
      }
   }

   public IObject MatchesIndex(IObject obj, Func<IObject, int, IObject> getter)
   {
      var _result = isMatch(obj.AsString);
      if (_result is (true, var result))
      {
         return getter(obj, result.Index);
      }
      else
      {
         return KNil.NilValue;
      }
   }

   public void MatchesIndex(IObject obj, Action<IObject, int, IObject> setter, IObject value)
   {
      var _result = isMatch(obj.AsString);
      if (_result is (true, var result))
      {
         setter(obj, result.Index, value);
      }
   }

   public IObject Matches(string input)
   {
      var self = this;
      var _result = isMatch(input);

      if (global)
      {
         if (_result is (true, var result))
         {
            return new KArray(result
               .Select(m => new RegexMatch(m, self.nameToIndex(result), self.indexToName(result), input.Keep(m.Index),
                  input.Drop(m.Index + m.Length), input))
               .Select(m => getMatchOrText(m, self.textOnly)).ToArray());
         }
         else
         {
            return KArray.Empty;
         }
      }
      else if (isMatch(input) is (true, var result2))
      {
         var match = result2.GetMatch(0);
         var regexMatch = new RegexMatch(match, self.nameToIndex(result2), self.indexToName(result2), input.Keep(match.Index),
            input.Drop(match.Index + match.Length), input);
         for (var index = 0; index < match.Groups.Length; index++)
         {
            setVariable(index, match.Groups[index].Text);
         }

         var prefix = input.Keep(result2.Index);
         var suffix = input.Drop(result2.Index + result2.Length);
         setVariable("prefix", prefix);
         setVariable("suffix", suffix);

         return Some.Object(getMatchOrText(regexMatch, self.textOnly));
      }
      else
      {
         return KNil.NilValue;
      }
   }

   public IObject Matches(string input, Lambda lambda)
   {
      var self = this;
      var _result = isMatch(input);

      if (global)
      {
         if (_result is (true, var result))
         {
            RegexMatch[] enumerable =
            [
               .. result.Select(m => new RegexMatch(m, self.nameToIndex(result), self.indexToName(result), input.Keep(m.Index),
                  input.Drop(m.Index + m.Length), input))
            ];
            List<IObject> returns = [];
            foreach (var match in enumerable)
            {
               if (lambda.Invoke(match).IsTrue)
               {
                  returns.Add(getMatchOrText(match, self.textOnly));
               }
            }

            return new KArray([.. returns]);
         }
         else
         {
            return KArray.Empty;
         }
      }
      else if (isMatch(input) is (true, var result2))
      {
         var match = result2.GetMatch(0);
         var regexMatch = new RegexMatch(match, self.nameToIndex(result2), self.indexToName(result2), input.Keep(match.Index),
            input.Drop(match.Index + match.Length), input);
         for (var index = 0; index < match.Groups.Length; index++)
         {
            setVariable(index, match.Groups[index].Text);
         }

         if (!lambda.Invoke(regexMatch).IsTrue)
         {
            return KNil.NilValue;
         }

         var prefix = input.Keep(result2.Index);
         var suffix = input.Drop(result2.Index + result2.Length);
         setVariable("prefix", prefix);
         setVariable("suffix", suffix);

         return Some.Object(getMatchOrText(regexMatch, self.textOnly));
      }
      else
      {
         return KNil.NilValue;
      }
   }

   public KBoolean NotMatches(string input) => !isMatch(input);

   private Core.Matching.Pattern getFixedPattern() => getPattern(originalPattern).WithMultiline(multiline).WithIgnoreCase(ignoreCase);

   public KString Replace(string input, string replacement)
   {
      var fixedPattern = getFixedPattern();
      if (global)
      {
         return input.Substitute(fixedPattern, replacement);
      }
      else
      {
         return input.Substitute(getPattern(originalPattern), replacement, 1);
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
            builder.Append(input.AsSpan(lastIndex, match.Index - lastIndex));
            var regexMatch = new RegexMatch(match, nameToIndex(result), indexToName(result), input.Keep(match.Index),
               input.Drop(match.Index + match.Length), input);
            var replacement = lambda.Invoke(regexMatch);
            builder.Append(replacement.AsString);
            lastIndex = match.Index + match.Length;
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
            return KNil.NilValue;
         }
      }
      else
      {
         return KNil.NilValue;
      }
   }

   public KArray FindAll(string input)
   {
      if (input.Matches(getFixedPattern()) is (true, var result))
      {
         return new KArray(result.Matches.Select(m => Int.IntObject(m.Index)));
      }
      else
      {
         return KArray.Empty;
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
            var resultText = match.Text.Substitute(getPattern(originalPattern), replacement);
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

   public KArray Split(string input) => new(input.Unjoin(getFixedPattern()).Select(KString.StringObject));

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
      Regex regex => new Regex(originalPattern + regex.originalPattern, global, textOnly),
      KString str => new Regex(getPattern(originalPattern).Regex + str.Value, ignoreCase, multiline, global, textOnly),
      _ => new Regex(getPattern(originalPattern).Regex + obj.AsString, ignoreCase, multiline, global, textOnly)
   };

   public Regex Concatenate(string otherPattern) => new(getPattern(originalPattern).Regex + otherPattern, ignoreCase, multiline, global, textOnly);

   public Optional<Match> MatchOne(string input) => getFixedPattern().MatchedBy(input).Map(r => r.Matches[0]);

   public IObject PendingRegex(KString input) => new PendingRegex(this, input);

   public string Pattern => getPattern(originalPattern).Regex;

   public bool IgnoreCase => ignoreCase;

   public bool Multiline => multiline;

   public bool Global => global;

   public bool Equals(Regex other) => getPattern(originalPattern).Equals(other.getPattern(originalPattern)) && ignoreCase == other.ignoreCase &&
      multiline == other.multiline &&
      global == other.global && textOnly == other.textOnly && nameToIndex.Equals(other.nameToIndex);

   public override bool Equals(object? obj) => obj is Regex other && Equals(other);

   public override int GetHashCode() => HashCode.Combine(originalPattern, ignoreCase, multiline, global, textOnly, nameToIndex);

   public IObject Accept(IObject obj) => obj switch
   {
      KString s => Matches(s.Value),
      _ => Matches(obj.AsString)
   };

   public static bool operator ==(Regex left, Regex right) => left.Equals(right);

   public static bool operator !=(Regex left, Regex right) => !left.Equals(right);

   public KArray Scan(string input)
   {
      List<IObject> list = [];
      if (global)
      {
         foreach (var match in input.AllMatches(getPattern(originalPattern)))
         {
            List<IObject> innerList = [];
            foreach (var group in match.Groups.Skip(1))
            {
               innerList.Add(KString.StringObject(group.Text));
            }

            var kArray = new KArray(innerList);
            list.Add(kArray);
         }
      }
      else if (input.Matches(getPattern(originalPattern)) is (true, var result))
      {
         foreach (var group in result.Groups(0).Skip(1))
         {
            list.Add(KString.StringObject(group));
         }
      }

      return new KArray(list);
   }

   public KString SplitMapJoin(string input, Lambda onMatch, Lambda onNonMatch)
   {
      var _result = input.Matches(getPattern(originalPattern));
      if (_result is (true, var result))
      {
         var builder = new StringBuilder();
         var index = 0;
         foreach (var match in result.Matches)
         {
            var prefix = input.Keep(result.Index);
            var newPrefix = onNonMatch.Invoke((KString)prefix).AsString;
            builder.Append(newPrefix);
            index += prefix.Length;

            var matchText = (KString)match.Text;
            var mapped = onMatch.Invoke(matchText).AsString;
            builder.Append(mapped);
            index += matchText.Value.Length;
         }

         var suffix = input.Drop(index);
         var newSuffix = onNonMatch.Invoke((KString)suffix).AsString;
         builder.Append(newSuffix);

         return builder.ToString();
      }
      else
      {
         return input;
      }
   }

   public Regex WithIgnoreCase(bool ignoreCase) => new(getPattern(originalPattern).WithIgnoreCase(ignoreCase), global, textOnly);

   public Regex WithMultiline(bool multiline) => new(getPattern(originalPattern).WithMultiline(multiline), global, textOnly);

   public Regex WithGlobal(bool global) => new(getPattern(originalPattern), global, textOnly);

   private void setVariable(int index, string text) => setVariable(index.ToString(), text);

   private void setVariable(string name, string text)
   {
      var machine = Machine.Current.Value;
      var fieldName = $"`{name}";
      machine.CurrentFrame.Fields.NewOrAssign(fieldName, FieldType.Assignment, KString.StringObject(text));
   }
}