using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Packages;

public class FileClass : BaseClass
{
   public override string Name => "File";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();

      messages["text".get()] = (obj, _) => function<File>(obj, f => f.Text);
      messages["lines".get()] = (obj, _) => function<File>(obj, f => new Iterator(f.Lines));
      messages["folder".get()] = (obj, _) => function<File>(obj, f => f.Folder);
      messages["name".get()] = (obj, _) => function<File>(obj, f => f.Name);
      messages["extension".get()] = (obj, _) => function<File>(obj, f => f.Extension);
      messages["nameExtension".get()] = (obj, _) => function<File>(obj, f => f.NameExtension);
      messages["fullPath".get()] = (obj, _) => function<File>(obj, f => f.FullPath);
   }

   public override IObject DefaultValue => new File("");
}