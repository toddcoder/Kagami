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

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["parse(_<String>,global:_<Boolean>,textOnly:_<Boolean>)"] = (_, msg) => parseRegex(msg.Arguments);
      classMessages["parse(_<String>)"] = (_, msg) => parseRegex(msg.Arguments);
   }

   protected static IObject parseRegex(Arguments arguments)
   {
      try
      {
         string pattern;
         bool global;
         bool textOnly;

         switch (arguments.Length)
         {
            case 3:
               pattern = ((KString)arguments[0]).Value;
               global = ((KBoolean)arguments[1]).Value;
               textOnly = ((KBoolean)arguments[2]).Value;
               break;
            case 1:
               pattern = ((KString)arguments[0]).Value;
               global = false;
               textOnly = false;
               break;
            default:
               return new Failure("Only 1 or 3 parameters allowed");
         }

         return Success.Object(new Regex(pattern, global, textOnly));
      }
      catch (Exception exception)
      {
         return new Failure(exception.Message);
      }
   }

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("TextFinding");
}