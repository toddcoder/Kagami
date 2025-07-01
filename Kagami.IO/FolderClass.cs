using Kagami.Library;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.IO;

public class FolderClass : BaseClass
{
   public override string Name => "Folder";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();

      messages["files".get()] = (obj, _) => function<Folder>(obj, f => new Iterator(new KArray(f.Files)));
      messages["folders".get()] = (obj, _) => function<Folder>(obj, f => new Iterator(new KArray(f.Folders)));
   }
}