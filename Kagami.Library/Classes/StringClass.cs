using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Objects;
using Standard.Types.Enumerables;
using static Kagami.Library.Classes.ClassFunctions;
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
         formatMessage<String>();
         sliceableMessages();
         compareMessages();
         rangeMessages();

         messages["~"] = (obj, msg) => function<String, IObject>(obj, msg, (s1, s2) => s1.Concatenate(s2.AsString));
         messages["[]"] = (obj, msg) => function<String, Int>(obj, msg, (s, i) => s[i.Value]);
         messages["length".get()] = (obj, msg) => function<String>(obj, s => s.Length);
         messages["upper"] = (obj, msg) => function<String>(obj, s => s.Upper());
         messages["lower"] = (obj, msg) => function<String>(obj, s => s.Lower());
         messages["title"] = (obj, msg) => function<String>(obj, s => s.Title());
         messages["capital"] = (obj, msg) => function<String>(obj, s => s.Title());
         messages["leftIs"] = (obj, msg) => function<String, String>(obj, msg, (s1, s2) => s1.IsPrefix(s2.Value));
         messages["rightIs"] = (obj, msg) => function<String, String>(obj, msg, (s1, s2) => s1.IsSuffix(s2.Value));
         messages["in"] = (obj, msg) => function<String, IObject>(obj, msg, (s1, s2) => s1.In(s2));
         messages["notIn"] = (obj, msg) => function<String, IObject>(obj, msg, (s1, s2) => s1.NotIn(s2));
         messages["replace".Function("find", "new")] =
            (obj, msg) => function<String, String, String>(obj, msg, (s, o, n) => s.Replace(o.Value, n.Value));
         messages["lstrip"] = (obj, msg) => function<String>(obj, s => s.LStrip());
         messages["rstrip"] = (obj, msg) => function<String>(obj, s => s.RStrip());
         messages["strip"] = (obj, msg) => function<String>(obj, s => s.Strip());
         messages["center".Function("width", "padding")] = (obj, msg) =>
            function<String, Int, Char>(obj, msg, (s, w, p) => s.Center(w.Value, p.Value));
         messages["center"] = (obj, msg) => function<String, Int>(obj, msg, (s, w) => s.Center(w.Value));
         messages["ljust".Function("width", "padding")] = (obj, msg) =>
            function<String, Int, Char>(obj, msg, (s, w, p) => s.LJust(w.Value, p.Value));
         messages["ljust"] = (obj, msg) => function<String, Int>(obj, msg, (s, w) => s.LJust(w.Value));
         messages["rjust".Function("width", "padding")] = (obj, msg) =>
            function<String, Int, Char>(obj, msg, (s, w, p) => s.RJust(w.Value, p.Value));
         messages["rjust"] = (obj, msg) => function<String, Int>(obj, msg, (s, w) => s.RJust(w.Value));
         messages["isEmpty".get()] = (obj, msg) => function<String>(obj, s => s.IsEmpty);
         messages["isNotEmpty".get()] = (obj, msg) => function<String>(obj, s => s.IsNotEmpty);
         messages["isAlphaDigit".get()] = (obj, msg) => function<String>(obj, s => s.IsAlphaDigit);
         messages["isAlpha".get()] = (obj, msg) => function<String>(obj, s => s.IsAlpha);
         messages["isDigit".get()] = (obj, msg) => function<String>(obj, s => s.IsDigit);
         messages["isLower".get()] = (obj, msg) => function<String>(obj, s => s.IsLower);
         messages["isUpper".get()] = (obj, msg) => function<String>(obj, s => s.IsUpper);
         messages["isSpace".get()] = (obj, msg) => function<String>(obj, s => s.IsSpace);
         messages["isTitle".get()] = (obj, msg) => function<String>(obj, s => s.IsTitle);
         messages["translate".Function("from", "to")] = (obj, msg) =>
            function<String, String, String>(obj, msg, (s, f, t) => s.Translate(f.Value, t.Value));
         messages["truncate".Function("width", "ellipses")] = (obj, msg) =>
            function<String, Int, Boolean>(obj, msg, (s, w, e) => s.Truncate(w.Value, e.Value));
         messages["truncate".Function("width")] = (obj, msg) => function<String, Int>(obj, msg, (s, w) => s.Truncate(w.Value));
         messages["find".Function("string", "startAt", "reverse")] = (obj, msg) =>
            function<String, String, Int, Boolean>(obj, msg, (s, u, i, r) => s.Find(u.Value, i.Value, r.Value));
         messages["find".Function("string", "startAt")] = (obj, msg) =>
            function<String, String, Int>(obj, msg, (s, u, i) => s.Find(u.Value, i.Value, false));
         messages["find".Function("string", "reverse")] = (obj, msg) =>
            function<String, String, Boolean>(obj, msg, (s, u, r) => s.Find(u.Value, 0, r.Value));
         messages["find"] = (obj, msg) =>
            function<String, String>(obj, msg, (s, u) => s.Find(u.Value, 0, false));
         messages["find".Function("all")] = (obj, msg) =>
            function<String, String>(obj, msg, (s, u) => s.FindAll(u.Value));
         messages["int"] = (obj, msg) => function<String>(obj, s => s.Int());
         messages["float"] = (obj, msg) => function<String>(obj, s => s.Float());
         messages["byte"] = (obj, msg) => function<String>(obj, s => s.Byte());
         messages["long"] = (obj, msg) => function<String>(obj, s => s.Long());
         messages["split".Function("regex")] = (obj, msg) => function<String, Regex>(obj, msg, (s, r) => s.SplitRegex(r));
         messages["split".Function("on")] = (obj, msg) => function<String, String>(obj, msg, (s1, s2) => s1.SplitOn(s2.Value));
         messages["-"] = (obj, msg) => function<String, String>(obj, msg, (s1, s2) => s1.Subtract(s2.Value));
      }

      public override void RegisterClassMessages()
      {
         base.RegisterClassMessages();

         classMessages["clrf".get()] = (cls, msg) => (String)"\r\n";
         classMessages["lcase".get()] = (cls, msg) => (String)"abcdefghijklmnopqrstuvwxyz";
         classMessages["ucase".get()] = (cls, msg) => (String)"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
         classMessages["alpha".get()] = (cls, msg) => (String)"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
         classMessages["digits".get()] = (cls, msg) => (String)"0123456789";
         classMessages["punctuation".get()] = (cls, msg) => (String)"!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
      }

      public IObject Revert(IEnumerable<IObject> list) => String.StringObject(list.Select(i => i.AsString).Listify(""));
   }
}