using Kagami.Library;
using Kagami.Library.Classes;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Drawing
{
   public class SizeClass : BaseClass
   {
      public override string Name => "Size";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["width".get()] = (obj, _) => function<Size>(obj, s => s.Width);
         messages["height".get()] = (obj, _) => function<Size>(obj, s => s.Height);
         messages["+"] = (obj, msg) => function<Size, Size>(obj, msg, (s1, s2) => s1.Add(s2));
         messages["-"] = (obj, msg) => function<Size, Size>(obj, msg, (s1, s2) => s1.Subtract(s2));
         messages["isEmpty".get()] = (obj, _) => function<Size>(obj, s => s.IsEmpty);
      }
   }
}