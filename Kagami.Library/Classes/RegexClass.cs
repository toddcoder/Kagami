using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class RegexClass : BaseClass, IEquivalentClass
{
   public override string Name => "Regex";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messages["matches(_<String>)"] = (obj, msg) => function<Regex, KString>(obj, msg, (r, s) => r.Matches(s.Value));
      messages["notMatches(_<String>)"] = (obj, msg) => function<Regex, KString>(obj, msg, (r, s) => r.NotMatches(s.Value));
      messages["isMatch(_<String>)"] = (obj, msg) => function<Regex, KString>(obj, msg, (r, s) => r.IsMatch(s.Value));
      messages["replace(_<String>, _<String>)"] = (obj, msg) =>
         function<Regex, KString, KString>(obj, msg, (r, s1, s2) => r.Replace(s1.Value, s2.Value));
      messages["replace(_<String>,_<Lambda>)"] = (obj, msg) => function<Regex, KString, Lambda>(obj, msg, (r, s, l) => r.Replace(s.Value, l));
      messages["split(_)"] = (obj, msg) => function<Regex, KString>(obj, msg, (r, s) => r.Split(s.Value));
      messages["~(_)"] = (obj, msg) => function<Regex, IObject>(obj, msg, (r1, r2) => r1.Concatenate(r2));
      messages["/(_<String>)"] = (obj, msg) => function<Regex, KString>(obj, msg, (r, s) => r.PendingRegex(s));
   }

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("TextFinding");
}