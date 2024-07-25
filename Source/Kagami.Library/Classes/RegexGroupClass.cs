using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class RegexGroupClass : BaseClass
   {
      public override string Name => "RegexGroup";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["text".get()] = (obj, _) => function<RegexGroup>(obj, g => g.Text);
         messages["index".get()] = (obj, _) => function<RegexGroup>(obj, g => g.Index);
         messages["length".get()] = (obj, _) => function<RegexGroup>(obj, g => g.Length);
      }
   }
}