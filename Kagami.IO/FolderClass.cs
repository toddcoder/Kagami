using System.Linq;
using Kagami.Library;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.IO
{
   public class FolderClass : BaseClass
   {
      public override string Name => "Folder";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         collectionMessages();

         messages["files".get()] = (obj, msg) =>
            function<Folder>(obj, f => new Tuple(f.Files.Select(file => (IObject)file).ToArray()));
         messages["folders".get()] = (obj, msg) =>
            function<Folder>(obj, f => new Tuple(f.Folders.Select(folder => (IObject)folder).ToArray()));
      }
   }
}