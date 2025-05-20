using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class RegexMatchClass : BaseClass
   {
      public override string Name => "RegexMatch";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["text".get()] = (obj, _) => function<RegexMatch>(obj, m => m.Text);
         messages["index".get()] = (obj, _) => function<RegexMatch>(obj, m => m.Index);
         messages["length".get()] = (obj, _) => function<RegexMatch>(obj, m => m.Length);
         messages["groups".get()] = (obj, _) => function<RegexMatch>(obj, m => m.Groups);
         messages["[](_<Int>)"] = (obj, msg) => function<RegexMatch, Int>(obj, msg, (m, i) => m[i.Value]);
         messages["[](_<String>)"] = (obj, msg) => function<RegexMatch, KString>(obj, msg, (m, s) => m[s.Value]);
      }
   }
}