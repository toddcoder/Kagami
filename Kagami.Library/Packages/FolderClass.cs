using Core.Computers;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Packages;

public class FolderClass : BaseClass
{
   public override string Name => "Folder";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();

      messages["files".get()] = (obj, _) => function<Folder>(obj, f => (IObject)f.Files);
      messages["folders".get()] = (obj, _) => function<Folder>(obj, f => (IObject)f.Folders);
      messages["fullPath".get()] = (obj, _) => function<Folder>(obj, f => (KString)f.AsString);
      messages["~(_)"] = (obj, msg) => function<Folder, KString>(obj, msg, (f, n) => f.Combine(n.Value));
   }

   public override IObject DefaultValue => new Folder(FolderName.Temp.FullPath);
}