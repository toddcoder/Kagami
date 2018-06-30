using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class RegexClass : BaseClass
   {
      public override string Name => "Regex";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["match".Function("input")] = (obj, msg) => function<Regex, String>(obj, msg, (r, s) => r.Match(s.Value));
         messages["isMatch"] = (obj, msg) => function<Regex, String>(obj, msg, (r, s) => r.IsMatch(s.Value));
         messages["replace".Function("", "with")] = (obj, msg) =>
            function<Regex, String, String>(obj, msg, (r, s1, s2) => r.Replace(s1.Value, s2.Value));
         messages["matchString"] = (obj, msg) => function<Regex, String>(obj, msg, (r, s) => r.MatchString(s.Value));
         messages["split"] = (obj, msg) => function<Regex, String>(obj, msg, (r, s) => r.Split(s.Value));
         messages["~"] = (obj, msg) => function<Regex, IObject>(obj, msg, (r1, r2) => r1.Concatenate(r2));
      }
   }
}