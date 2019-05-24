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

         messages["text".get()] = (obj, msg) => function<RegexMatch>(obj, m => m.Text);
         messages["index".get()] = (obj, msg) => function<RegexMatch>(obj, m => m.Index);
         messages["length".get()] = (obj, msg) => function<RegexMatch>(obj, m => m.Length);
         messages["groups".get()] = (obj, msg) => function<RegexMatch>(obj, m => m.Groups);
         messages["[](_)"] = (obj, msg) => function<RegexMatch, Int>(obj, msg, (m, i) => m[i.Value]);
      }
   }
}