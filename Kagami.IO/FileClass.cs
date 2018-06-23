using Kagami.Library;
using Kagami.Library.Classes;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.IO
{
   public class FileClass : BaseClass
   {
      public override string Name => "File";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         collectionMessages();

         messages["text".get()] = (obj, msg) => function<File>(obj, f => f.Text);
         messages["lines".get()] = (obj, msg) => function<File>(obj, f => f.Lines);
      }
   }
}