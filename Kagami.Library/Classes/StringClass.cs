using System.Numerics;
using Kagami.Library.Objects;
using Core.Enumerables;
using Core.Objects;
using static Kagami.Library.Classes.ClassFunctions;

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
      messages["fields()"] = (obj, _) => function<KString>(obj, s => s.Fields);
      messages["words(at:_<Int>)"] = (obj, msg) => function<KString, Int>(obj, msg, (s, i) => s.Words(i.Value));
      messages["words()"] = (obj, _) => function<KString>(obj, s => s.Words());
      messages["<<(_)"] = (obj, msg) => function<KString, IObject>(obj, msg, (s, o) => s.Append(o));
      messages["mutable()"] = (obj, _) => function<KString>(obj, s => s.Mutable());
      messages["succ()"] = (obj, _) => function<KString>(obj, s => s.Succ());
      messages["pred()"] = (obj, _) => function<KString>(obj, s => s.Pred());
      messages["range()"] = (obj, _) => function<KString>(obj, s => s.Range());
      messages["replace(_<String>,_<String>)"] = (obj, msg) => function<KString, KString, KString>(obj, msg, (s1, s2, s3) => s1.Replace(s2, s3));
      messages["replace(_<Regex>,_<String>)"] = (obj, msg) => function<KString, Regex, KString>(obj, msg, (s, r, t) => r.Replace(s.Value, t.Value));
      messages["replace".Selector("<Regex>", "<Lambda>")] =
         (obj, msg) => function<KString, Regex, Lambda>(obj, msg, (s, r, l) => r.Replace(s.Value, l));
      messages["replace(_<Dictionary>)"] = (obj, msg) => function<KString, Dictionary>(obj, msg, (s, d) => s.ReplaceAll(d));
      messages["squeeze()"] = (obj, _) => function<KString>(obj, s => s.Squeeze());
      messages["isMatch(_<Regex>)"] = (obj, msg) => function<KString, Regex>(obj, msg, (s, r) => r.IsMatch(s.Value));
      messages["-(_)"] = (obj, msg) => function<KString, KString>(obj, msg, (s1, s2) => s1.Subtract(s2.Value));
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
   }

   public IObject Revert(IEnumerable<IObject> list) => KString.StringObject(list.Select(i => i.AsString).ToString(""));

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection", "TextFinding");

   public override bool AssignCompatible(BaseClass otherClass)
   {
      return base.AssignCompatible(otherClass) || otherClass.Name == "MutString";
   }

   public override IObject DefaultValue => KString.Empty;
}