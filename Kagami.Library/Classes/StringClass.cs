using System.Globalization;
using System.Numerics;
using System.Text;
using Core.Collections;
using Kagami.Library.Objects;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using CollectionFunctions = Kagami.Library.Objects.CollectionFunctions;
using Complex = Kagami.Library.Objects.Complex;

namespace Kagami.Library.Classes;

public class StringClass : BaseClass, ICollectionClass
{
   public override string Name => "String";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();
      sliceableMessages();
      compareMessages();
      rangeMessages();
      textFindingMessages();
      indexedMessages();
      acceptingMessages();

      messages["~(_)"] = (obj, msg) => function<KString, IObject>(obj, msg, (s1, s2) => s1.Concatenate(s2.AsString));
      messages["+(_)"] = (obj, msg) => function<KString, KString>(obj, msg, (s1, s2) => s1.Concatenate(s2.AsString));
      messages["[](_)"] = (obj, msg) => function<KString, IObject>(obj, msg, getIndexed);
      messages["length".get()] = (obj, _) => function<KString>(obj, s => s.Length);
      messages["upper()"] = (obj, _) => function<KString>(obj, s => s.Upper());
      messages["lower()"] = (obj, _) => function<KString>(obj, s => s.Lower());
      messages["title()"] = (obj, _) => function<KString>(obj, s => s.Title());
      messages["upper1()"] = (obj, _) => function<KString>(obj, s => s.Upper1());
      messages["lower1()"] = (obj, _) => function<KString>(obj, s => s.Lower1());
      messages["camel()"] = (obj, _) => function<KString>(obj, s => s.Camel());
      messages["pascal()"] = (obj, _) => function<KString>(obj, s => s.Pascal());
      messages["startsWith(_)"] = (obj, msg) => function<KString, KString>(obj, msg, (s1, s2) => s1.IsPrefix(s2.Value));
      messages["endsWith(_)"] = (obj, msg) => function<KString, KString>(obj, msg, (s1, s2) => s1.IsSuffix(s2.Value));
      messages["in(_)"] = (obj, msg) => function<KString, IObject>(obj, msg, (s1, s2) => s1.In(s2));
      messages["notIn(_)"] = (obj, msg) => function<KString, IObject>(obj, msg, (s1, s2) => s1.NotIn(s2));
      messages["lstrip()"] = (obj, _) => function<KString>(obj, s => s.LStrip());
      messages["rstrip()"] = (obj, _) => function<KString>(obj, s => s.RStrip());
      messages["strip()"] = (obj, _) => function<KString>(obj, s => s.Strip());
      messages["center(_<Int>,_<Char>)"] =
         (obj, msg) => function<KString, Int, KChar>(obj, msg, (s, w, p) => s.Center(w.Value, p.Value));
      messages["center(_<Int>)"] = (obj, msg) => function<KString, Int>(obj, msg, (s, w) => s.Center(w.Value));
      messages["ljust(_<Int>,_<Char>)"] = (obj, msg) =>
         function<KString, Int, KChar>(obj, msg, (s, w, p) => s.LJust(w.Value, p.Value));
      messages["ljust(_<Int>)"] = (obj, msg) => function<KString, Int>(obj, msg, (s, w) => s.LJust(w.Value));
      messages["rjust(_<Int>,_<Char>)"] = (obj, msg) =>
         function<KString, Int, KChar>(obj, msg, (s, w, p) => s.RJust(w.Value, p.Value));
      messages["rjust(_<Int>)"] = (obj, msg) => function<KString, Int>(obj, msg, (s, w) => s.RJust(w.Value));
      messages["isEmpty".get()] = (obj, _) => function<KString>(obj, s => s.IsEmpty);
      messages["isNotEmpty".get()] = (obj, _) => function<KString>(obj, s => s.IsNotEmpty);
      messages["isAlphaDigit".get()] = (obj, _) => function<KString>(obj, s => s.IsAlphaDigit);
      messages["isAlpha".get()] = (obj, _) => function<KString>(obj, s => s.IsAlpha);
      messages["isDigit".get()] = (obj, _) => function<KString>(obj, s => s.IsDigit);
      messages["isLower".get()] = (obj, _) => function<KString>(obj, s => s.IsLower);
      messages["isUpper".get()] = (obj, _) => function<KString>(obj, s => s.IsUpper);
      messages["isSpace".get()] = (obj, _) => function<KString>(obj, s => s.IsSpace);
      messages["isTitle".get()] = (obj, _) => function<KString>(obj, s => s.IsTitle);
      messages["translate(from:_<String>,to:_<String>)"] = (obj, msg) =>
         function<KString, KString, KString>(obj, msg, (s, f, t) => s.Translate(f.Value, t.Value));
      messages["translate(_<Dictionary>)"] = (obj, msg) => function<KString, Dictionary>(obj, msg, (s, d) => s.Translate(d, false));
      messages["translate(_<Dictionary>,omit:_<Boolean>)"] =
         (obj, msg) => function<KString, Dictionary, KBoolean>(obj, msg, (s, d, o) => s.Translate(d, o.Value));
      messages["truncate".Selector("<Int>", "<Boolean>")] = (obj, msg) =>
         function<KString, Int, KBoolean>(obj, msg, (s, w, e) => s.Truncate(w.Value, e.Value));
      messages["truncate(_,_)"] = (obj, msg) => function<KString, Int>(obj, msg, (s, w) => s.Truncate(w.Value));
      messages["int()"] = (obj, _) => function<KString>(obj, s => s.Int());
      messages["float()"] = (obj, _) => function<KString>(obj, s => s.Float());
      messages["byte()"] = (obj, _) => function<KString>(obj, s => s.Byte());
      messages["long()"] = (obj, _) => function<KString>(obj, s => s.Long());
      messages["-(_)"] = (obj, msg) => function<KString, KString>(obj, msg, (s1, s2) => s1.Subtract(s2.Value));
      messages["get()"] = (obj, _) => function<KString>(obj, s => s.Get());
      messages["set()"] = (obj, _) => function<KString>(obj, s => s.Set());
      messages["swapCase()"] = (obj, _) => function<KString>(obj, s => s.SwapCase());
      messages["fields".get()] = (obj, _) => function<KString>(obj, s => s.Fields());
      messages["fields(_<Regex>)"] = (obj, msg) => function<KString, Regex>(obj, msg, (s, regex) => s.Fields(regex));
      messages["field(at:_<Int>)"] = (obj, msg) => function<KString, Int>(obj, msg, (s, i) => s.Field(i.Value));
      messages["words(at:_<Int>)"] = (obj, msg) => function<KString, Int>(obj, msg, (s, i) => s.Words(i.Value));
      messages["words()"] = (obj, _) => function<KString>(obj, s => s.Words());
      messages["word(at:_<Int>)"] = (obj, msg) => function<KString, Int>(obj, msg, (s, i) => s.Word(i.Value));
      messages["<<(_)"] = (obj, msg) => function<KString, IObject>(obj, msg, (s, o) => s.Append(o));
      messages["mutable()"] = (obj, _) => function<KString>(obj, s => s.Mutable());
      messages["succ()"] = (obj, _) => function<KString>(obj, s => s.Succ());
      messages["pred()"] = (obj, _) => function<KString>(obj, s => s.Pred());
      messages["range()"] = (obj, _) => function<KString>(obj, s => s.Range());
      messages["replace(_<String>,_<String>)"] =
         (obj, msg) => function<KString, KString, KString>(obj, msg, (s1, s2, s3) => s1.Replace(s2, s3, false));
      messages["replace(_<String>,_<String>,ignoreCase:_<Boolean>)"] = (obj, msg) =>
         function<KString, KString, KString, KBoolean>(obj, msg, (s1, s2, s3, b) => s1.Replace(s2, s3, b.Value));
      messages["replace(_<Regex>,_<String>)"] = (obj, msg) => function<KString, Regex, KString>(obj, msg, (s, r, t) => r.Replace(s.Value, t.Value));
      messages["replace".Selector("<Regex>", "<Lambda>")] =
         (obj, msg) => function<KString, Regex, Lambda>(obj, msg, (s, r, l) => r.Replace(s.Value, l));
      messages["replace(_<Dictionary>)"] = (obj, msg) => function<KString, Dictionary>(obj, msg, (s, d) => s.ReplaceAll(d));
      messages["squeeze()"] = (obj, _) => function<KString>(obj, s => s.Squeeze());
      messages["isMatch(_<Regex>)"] = (obj, msg) => function<KString, Regex>(obj, msg, (s, r) => r.IsMatch(s.Value));
      messages["-(_<String>)"] = (obj, msg) => function<KString, KString>(obj, msg, (s1, s2) => s1.Subtract(s2.Value));
      messages["-(_<Range>)"] = (obj, msg) => function<KString, KRange>(obj, msg, (s, r) => s.Subtract(r));
      messages["pad(left:_<Int>)"] = (obj, msg) => function<KString, Int>(obj, msg, (s, w) => s.PadLeft(w.Value));
      messages["pad(left:_<Int>,padding:_<Char>)"] = (obj, msg) => function<KString, Int, KChar>(obj, msg, (s, w, c) => s.PadLeft(w.Value, c.Value));
      messages["pad(right:_<Int>)"] = (obj, msg) => function<KString, Int>(obj, msg, (s, w) => s.PadRight(w.Value));
      messages["pad(right:_<Int>,padding:_<Char>)"] =
         (obj, msg) => function<KString, Int, KChar>(obj, msg, (s, w, c) => s.PadRight(w.Value, c.Value));
      messages["pad(center:_<Int>)"] = (obj, msg) => function<KString, Int>(obj, msg, (s, w) => s.PadCenter(w.Value));
      messages["pad(center:_<Int>,padding:_<Char>)"] =
         (obj, msg) => function<KString, Int, KChar>(obj, msg, (s, w, c) => s.PadCenter(w.Value, c.Value));
      messages["head".get()] = (obj, _) => function<KString>(obj, s => s.Head);
      messages["tail".get()] = (obj, _) => function<KString>(obj, s => s.Tail);
      messages["split(_<Regex>)"] = (obj, msg) => function<KString, Regex>(obj, msg, (s, r) => s.SplitRegex(r));
      messages["margin()"] = (obj, _) => function<KString>(obj, s => s.Margin());
      messages["parse(base:_<Int>)"] = (obj, msg) => function<KString, Int>(obj, msg, (s, i) => s.ParseBase(i.Value));
      messages["wordCase()"] = (obj, _) => function<KString>(obj, s => s.WordCase());
      messages["encode(_)"] = (obj, msg) => function<KString, KString>(obj, msg, (s, e) => s.Encode(e.Value));
      messages["/(_<Regex>)"] = (obj, msg) => function<KString, Regex>(obj, msg, (s, r) => r.PendingRegex(s));
      messages["i".get()] = (obj, _) => function<KString>(obj, s => (Int)s.Value.Value().Int32());
      messages["f".get()] = (obj, _) => function<KString>(obj, s => (Float)s.Value.Value().Double());
      messages["l".get()] = (obj, _) => function<KString>(obj, s => (Long)BigInteger.Parse(s.Value));
      messages["d".get()] = (obj, _) => function<KString>(obj, s => (KDecimal)s.Value.Value().Decimal());
      messages["scan(_<Regex>)"] = (obj, msg) => function<KString, Regex>(obj, msg, (s, r) => r.Scan(s.Value));
      messages["splitMapJoin(_<Regex>,onMatch:_<Lambda>,noMatch:_<Lambda>)"] = (obj, msg) =>
         function<KString, Regex, Lambda, Lambda>(obj, msg, (s, r, lm, lnm) => r.SplitMapJoin(s.Value, lm, lnm));
      messages["numberize()"] = (obj, _) => function<KString>(obj, s => s.Numberize());
      messages["lines".get()] = (obj, _) => function<KString>(obj, s => s.Lines());
      messages["split(by:_<Collection>,keepRest:_<Boolean>)"] = (obj, msg) =>
         function<KString, IObject, KBoolean>(obj, msg, (s, c, b) => s.SplitOn((ICollection)c, b.Value));
      messages["split(by:_<Collection>)"] = (obj, msg) => function<KString, IObject>(obj, msg, (s, c) => s.SplitOn((ICollection)c, false));
      registerMessage("assign(_,_)", (obj, message) => function<KString, IObject, IObject>(obj, message, replaceString));
      messages["expandTabs()"] = (obj, _) => function<KString>(obj, s => s.ExpandTabs());
      messages["expandTabs(_<Int>)"] = (obj, msg) => function<KString, Int>(obj, msg, (s, i) => s.ExpandTabs(i.Value));
      messages["read(_<String>)"] = (obj, msg) => function<KString, KString>(obj, msg, (s, f) => Read(f.Value, s.Value));
      messages["capitalize()"] = (obj, _) => function<KString>(obj, s => s.Capitalize());
      messages["insert(value:_<String>,at:_<Int>)"] =
         (obj, msg) => function<KString, KString, Int>(obj, msg, (s1, s2, i) => s1.Insert(s2.Value, i.Value));
      messages["delete(from:_<Int>,length:_<Int>)"] =
         (obj, msg) => function<KString, Int, Int>(obj, msg, (s1, i1, i2) => s1.Delete(i1.Value, i2.Value));
   }

   protected static IObject replaceString(KString kString, IObject possibleSkipTake, IObject source)
   {
      if (possibleSkipTake is SkipTake skipTake)
      {
         if (source is KString stringSource)
         {
            return kString.Assign(skipTake, stringSource);
         }
         else
         {
            throw fail("Source must be a string");
         }
      }
      else
      {
         throw fail("Index must be a skip and take");
      }
   }

   protected static IObject getIndexed(KString s, IObject i)
   {
      return CollectionFunctions.getIndexed(s, i, (str, index) => ((KString)str)[index], (str, list) => ((KString)str)[list]);
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["clrf".get()] = (_, _) => (KString)"\r\n";
      classMessages["lalpha".get()] = (_, _) => (KString)"abcdefghijklmnopqrstuvwxyz";
      classMessages["ualpha".get()] = (_, _) => (KString)"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      classMessages["alpha".get()] = (_, _) => (KString)"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
      classMessages["lvowels".get()] = (_, _) => (KString)"aeiou";
      classMessages["uvowels".get()] = (_, _) => (KString)"AEIOU";
      classMessages["lconsonants".get()] = (_, _) => (KString)"bcdfghjklmnpqrstvwxyz";
      classMessages["uconsonants".get()] = (_, _) => (KString)"BCDFGHJKLMNPQRSTVWXYZ";
      classMessages["digits".get()] = (_, _) => (KString)"0123456789";
      classMessages["punctuation".get()] = (_, _) => (KString)"!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
      registerClassMessage("translate(from:_<String>,to:_<String>)",
         (bc, msg) => classFunc<StringClass, KString, KString>(bc, msg, (_, s1, s2) => translation(s1.Value, s2.Value)));
   }

   public static Dictionary translation(string from, string to)
   {
      Hash<char, char> hash = [];
      var length = Math.Min(from.Length, to.Length);
      for (var i = 0; i < length; i++)
      {
         if (!hash.ContainsKey(from[i]))
         {
            hash[from[i]] = to[i];
         }
      }

      var objectHash = hash.ToHash(i => KChar.CharObject(i.Key), i => KChar.CharObject(i.Value));
      return new Dictionary(objectHash);
   }

   public IObject Revert(IEnumerable<IObject> list, Maybe<TypeConstraint> _typeConstraint) =>
      KString.StringObject(list.Select(i => i.AsString).ToString(""));

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Collection", "TextFinding");

   public override bool AssignCompatible(BaseClass otherClass)
   {
      return base.AssignCompatible(otherClass) || otherClass.Name == "MutString";
   }

   public override IObject DefaultValue => KString.Empty;

   public static IObject Read(string format, string source)
   {
      List<IObject> result = [];

      var i = 0;
      while (i < format.Length && source.IsNotEmpty())
      {
         switch (format[i])
         {
            case 'f' when getFloat() is (true, var floatValue):
            {
               result.Add(floatValue);
               break;
            }
            case 'd' when getDecimal() is (true, var decimalValue):
            {
               result.Add(decimalValue);
               break;
            }
            case 'c' when getComplex() is (true, var complexValue):
            {
               result.Add(complexValue);
               break;
            }
            case 'i' when getInt() is (true, var intValue):
            {
               result.Add(intValue);
               break;
            }
            case 'l' when getLong() is (true, var longValue):
            {
               result.Add(longValue);
               break;
            }
            case 'x' when getHex() is (true, var hexValue):
            {
               result.Add(hexValue);
               break;
            }
            case 'o' when getOctal() is (true, var octalValue):
            {
               result.Add(octalValue);
               break;
            }
            case 'b' when getBinary() is (true, var binaryValue):
            {
               result.Add(binaryValue);
               break;
            }
            case '.' when getCharacter() is (true, var characterValue):
            {
               result.Add(characterValue);
               break;
            }
            case 's' when getString() is (true, var stringValue):
            {
               result.Add(stringValue);
               break;
            }
            case 'w' when source.Matches(@"^(\s+); u") is (true, var wsResult):
            {
               result.Add((KString)wsResult.FirstGroup);
               source = source.Drop(wsResult.Length);
               i++;
               break;
            }
            case '/':
            {
               source = source.Drop(1);
               i++;
               break;
            }
            case 'z' when getLetters() is (true, var lettersValue):
            {
               result.Add(lettersValue);
               break;
            }
            case '9' when getDigits() is (true, var digitsValue):
            {
               result.Add(digitsValue);
               break;
            }
            default:
               return result.Count == 1 ? result[0] : new KArray(result);
         }
      }

      return result.Count == 1 ? result[0] : new KArray(result);

      Maybe<Float> getFloat()
      {
         if (source.Matches(@"^(\s*)(\d[\d_`]*\.\d[\d_`]*)(?:([eE])([-\+]?\d+))?(f)?; u") is (true, var result))
         {
            var floatSource = result.SecondGroup.Replace("_", "").Replace("`", "") + result.ThirdGroup + result.FourthGroup;
            var _float = floatSource.Maybe().Double().Map(d => (Float)d);
            if (_float)
            {
               source = source.Drop(result.Length);
               i++;
            }

            return _float;
         }
         else
         {
            return nil;
         }
      }

      Maybe<KDecimal> getDecimal()
      {
         if (source.Matches(@"^(\s*)(\d[\d_`]*\.\d[\d_`]*)(?:([eE])([-\+]?\d+))?(d); u") is (true, var result))
         {
            var decimalSource = result.SecondGroup.Replace("_", "").Replace("`", "") + result.ThirdGroup + result.FourthGroup;
            var _decimal = decimalSource.Maybe().Decimal().Map(d => (KDecimal)d);
            if (_decimal)
            {
               source = source.Drop(result.Length);
               i++;
            }

            return _decimal;
         }
         else
         {
            return nil;
         }
      }

      Maybe<Complex> getComplex()
      {
         if (source.Matches(@"^(\s*)(\d[\d_`]*\.\d[\d_`]*)(?:([eE])([-\+]?\d+))?(i); u") is (true, var result))
         {
            var complexSource = result.SecondGroup.Replace("_", "").Replace("`", "") + result.ThirdGroup + result.FourthGroup;
            if (System.Numerics.Complex.TryParse(complexSource, CultureInfo.InvariantCulture, out var complex))
            {
               source = source.Drop(result.Length);
               i++;

               return (Complex)complex;
            }
            else
            {
               return nil;
            }
         }
         else
         {
            return nil;
         }
      }

      Maybe<Int> getInt()
      {
         if (source.Matches(@"^(\s*)(\d[\d_`]*)(i)?\b; u") is (true, var result))
         {
            var intSource = result.SecondGroup.Replace("_", "").Replace("`", "");
            var _int = intSource.Maybe().Int32().Map(i => (Int)i);
            if (_int)
            {
               source = source.Drop(result.Length);
               i++;
            }

            return _int;
         }
         else
         {
            return nil;
         }
      }

      Maybe<Long> getLong()
      {
         if (source.Matches(@"^(\s*)(\d[\d_`]*)(L); u") is (true, var result))
         {
            var longSource = result.SecondGroup.Replace("_", "").Replace("`", "");
            if (BigInteger.TryParse(longSource, out var bigInteger))
            {
               source = source.Drop(result.Length);
               i++;

               return (Long)bigInteger;
            }
            else
            {
               return nil;
            }
         }
         else
         {
            return nil;
         }
      }

      Maybe<IObject> getHex()
      {
         if (source.Matches(@"^(\s*)(0x)([0-9a-fA-F][0-9a-fA-F_`]*)([Li])?\b; u") is (true, var result))
         {
            var hexSource = result.ThirdGroup;
            var type = result.FourthGroup;
            var number = convert(hexSource.ToLower().Replace("_", "").Replace("`", ""), 16, "0123456789abcdef");
            var _number = getNumber(type, number);
            if (_number)
            {
               source = source.Drop(result.Length);
               i++;

               return _number.Maybe();
            }
            else
            {
               return nil;
            }
         }
         else
         {
            var ch = source[0];
            var intValue = (int)ch;
            var hex = intValue.FormatAs("x");
            source = source.Drop(1);
            i++;

            return (KString)hex;
         }
      }

      Maybe<IObject> getOctal()
      {
         if (source.Matches(@"^(\s*)(0o)([0-7][0-7_`]*)([Li])?\b; u") is (true, var result))
         {
            var octalSource = result.ThirdGroup;
            var type = result.FourthGroup;
            var number = convert(octalSource.ToLower().Replace("_", "").Replace("`", ""), 8, "01234567");
            var _number = getNumber(type, number);
            if (_number)
            {
               source = source.Drop(result.Length);
               i++;

               return _number.Maybe();
            }
            else
            {
               return nil;
            }
         }
         else
         {
            var ch = source[0];
            var intValue = (int)ch;
            var octal = formatNumber(intValue, "o");
            source = source.Drop(1);
            i++;

            return (KString)octal;
         }
      }

      Maybe<IObject> getBinary()
      {
         if (source.Matches(@"^(\s*)(0b)([01][01_`]*)([Li])?\b; u") is (true, var result))
         {
            var binarySource = result.ThirdGroup;
            var type = result.FourthGroup;
            var number = convert(binarySource.ToLower().Replace("_", "").Replace("`", ""), 2, "01");
            var _number = getNumber(type, number);
            if (_number)
            {
               source = source.Drop(result.Length);
               i++;

               return _number.Maybe();
            }
            else
            {
               return nil;
            }
         }
         else
         {
            var ch = source[0];
            var intValue = (int)ch;
            var binary = formatNumber(intValue, "b");
            source = source.Drop(1);
            i++;

            return (KString)binary;
         }
      }

      Maybe<IObject> getCharacter()
      {
         var ch = KChar.CharObject(source[0]);
         source = source.Drop(1);
         i++;

         return ch.Some();
      }

      Maybe<KString> getString()
      {
         if (format.Drop(++i).Matches(@"(\d+); u") is (true, var result))
         {
            var _count = result.FirstGroup.Maybe().Int32();
            if (_count is (true, var count))
            {
               var @string = source.Keep(count);
               source = source.Drop(count);
               i += result.Length;

               return (KString)@string;
            }
            else
            {
               return nil;
            }
         }
         else
         {
            return nil;
         }
      }

      Maybe<KString> getLetters()
      {
         var builder = new StringBuilder();
         var j = 0;
         while (j < source.Length)
         {
            if (char.IsLetter(source[j]))
            {
               builder.Append(source[j]);
               j++;
            }
            else
            {
               break;
            }
         }

         i++;
         source = source.Drop(j);

         return (KString)builder.ToString();
      }

      Maybe<KString> getDigits()
      {
         var builder = new StringBuilder();
         var j = 0;
         while (j < source.Length)
         {
            if (char.IsDigit(source[j]))
            {
               builder.Append(source[j]);
               j++;
            }
            else
            {
               break;
            }
         }

         i++;
         source = source.Drop(j);

         return (KString)builder.ToString();
      }
   }
}