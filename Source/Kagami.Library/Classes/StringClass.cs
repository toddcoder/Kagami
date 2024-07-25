using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Objects;
using Core.Enumerables;
using static Kagami.Library.Classes.ClassFunctions;
using Boolean = Kagami.Library.Objects.Boolean;
using String = Kagami.Library.Objects.String;
using Char = Kagami.Library.Objects.Char;

namespace Kagami.Library.Classes
{
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

         messages["~(_)"] = (obj, msg) => function<String, IObject>(obj, msg, (s1, s2) => s1.Concatenate(s2.AsString));
         messages["[](_)"] = (obj, msg) => function<String, IObject>(obj, msg, getIndexed);
         messages["length".get()] = (obj, _) => function<String>(obj, s => s.Length);
         messages["upper()"] = (obj, _) => function<String>(obj, s => s.Upper());
         messages["lower()"] = (obj, _) => function<String>(obj, s => s.Lower());
         messages["title()"] = (obj, _) => function<String>(obj, s => s.Title());
         messages["upper1()"] = (obj, _) => function<String>(obj, s => s.Upper1());
         messages["lower1()"] = (obj, _) => function<String>(obj, s => s.Lower1());
         messages["startsWith(_)"] = (obj, msg) => function<String, String>(obj, msg, (s1, s2) => s1.IsPrefix(s2.Value));
         messages["endsWith(_)"] = (obj, msg) => function<String, String>(obj, msg, (s1, s2) => s1.IsSuffix(s2.Value));
         messages["in(_)"] = (obj, msg) => function<String, IObject>(obj, msg, (s1, s2) => s1.In(s2));
         messages["notIn(_)"] = (obj, msg) => function<String, IObject>(obj, msg, (s1, s2) => s1.NotIn(s2));
         messages["lstrip()"] = (obj, _) => function<String>(obj, s => s.LStrip());
         messages["rstrip()"] = (obj, _) => function<String>(obj, s => s.RStrip());
         messages["strip()"] = (obj, _) => function<String>(obj, s => s.Strip());
         messages["center(_<Int>,_<Char>)"] =
            (obj, msg) => function<String, Int, Char>(obj, msg, (s, w, p) => s.Center(w.Value, p.Value));
         messages["center(_<Int>)"] = (obj, msg) => function<String, Int>(obj, msg, (s, w) => s.Center(w.Value));
         messages["ljust(_<Int>,_<Char>)"] = (obj, msg) =>
            function<String, Int, Char>(obj, msg, (s, w, p) => s.LJust(w.Value, p.Value));
         messages["ljust(_<Int>)"] = (obj, msg) => function<String, Int>(obj, msg, (s, w) => s.LJust(w.Value));
         messages["rjust(_<Int>,_<Char>)"] = (obj, msg) =>
            function<String, Int, Char>(obj, msg, (s, w, p) => s.RJust(w.Value, p.Value));
         messages["rjust(_<Int>)"] = (obj, msg) => function<String, Int>(obj, msg, (s, w) => s.RJust(w.Value));
         messages["isEmpty".get()] = (obj, _) => function<String>(obj, s => s.IsEmpty);
         messages["isNotEmpty".get()] = (obj, _) => function<String>(obj, s => s.IsNotEmpty);
         messages["isAlphaDigit".get()] = (obj, _) => function<String>(obj, s => s.IsAlphaDigit);
         messages["isAlpha".get()] = (obj, _) => function<String>(obj, s => s.IsAlpha);
         messages["isDigit".get()] = (obj, _) => function<String>(obj, s => s.IsDigit);
         messages["isLower".get()] = (obj, _) => function<String>(obj, s => s.IsLower);
         messages["isUpper".get()] = (obj, _) => function<String>(obj, s => s.IsUpper);
         messages["isSpace".get()] = (obj, _) => function<String>(obj, s => s.IsSpace);
         messages["isTitle".get()] = (obj, _) => function<String>(obj, s => s.IsTitle);
         messages["translate".Selector("from:<String>", "to:<String>")] = (obj, msg) =>
            function<String, String, String>(obj, msg, (s, f, t) => s.Translate(f.Value, t.Value));
         messages["truncate".Selector("<Int>", "<Boolean>")] = (obj, msg) =>
            function<String, Int, Boolean>(obj, msg, (s, w, e) => s.Truncate(w.Value, e.Value));
         messages["truncate"] = (obj, msg) => function<String, Int>(obj, msg, (s, w) => s.Truncate(w.Value));
         messages["int"] = (obj, _) => function<String>(obj, s => s.Int());
         messages["float"] = (obj, _) => function<String>(obj, s => s.Float());
         messages["byte"] = (obj, _) => function<String>(obj, s => s.Byte());
         messages["long"] = (obj, _) => function<String>(obj, s => s.Long());
         messages["-"] = (obj, msg) => function<String, String>(obj, msg, (s1, s2) => s1.Subtract(s2.Value));
         messages["get"] = (obj, _) => function<String>(obj, s => s.Get());
         messages["set"] = (obj, _) => function<String>(obj, s => s.Set());
         messages["swapCase"] = (obj, _) => function<String>(obj, s => s.SwapCase());
         messages["fields".get()] = (obj, _) => function<String>(obj, s => s.Fields);
         messages["words(at:_<Int>)"] = (obj, msg) => function<String, Int>(obj, msg, (s, i) => s.Words(i.Value));
         messages["words()"] = (obj, _) => function<String>(obj, s => s.Words());
         messages["<<(_}"] = (obj, msg) => function<String, IObject>(obj, msg, (s, o) => s.Append(o));
         messages["mutable()"] = (obj, _) => function<String>(obj, s => s.Mutable());
      }

      protected static IObject getIndexed(String s, IObject i)
      {
         return CollectionFunctions.getIndexed(s, i, (str, index) => str[index], (str, list) => str[list]);
      }

      public override void RegisterClassMessages()
      {
         base.RegisterClassMessages();

         classMessages["clrf".get()] = (_, _) => (String)"\r\n";
         classMessages["lcase".get()] = (_, _) => (String)"abcdefghijklmnopqrstuvwxyz";
         classMessages["ucase".get()] = (_, _) => (String)"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
         classMessages["alpha".get()] = (_, _) => (String)"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
         classMessages["digits".get()] = (_, _) => (String)"0123456789";
         classMessages["punctuation".get()] = (_, _) => (String)"!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
      }

      public IObject Revert(IEnumerable<IObject> list) => String.StringObject(list.Select(i => i.AsString).ToString(""));

      public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection", "TextFinding");

      public override bool AssignCompatible(BaseClass otherClass)
      {
         return base.AssignCompatible(otherClass) || otherClass.Name == "MutString";
      }
   }
}