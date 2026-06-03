using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Packages;

public class FolderIterator : Iterator
{
   protected IIterator files;
   protected IIterator folders;

   public FolderIterator(Folder folder) : base(folder)
   {
      files = folder.Files;
      folders = folder.Folders;
   }

   public override Maybe<IObject> Next()
   {
      if (files.Next() is (true, var file))
      {
         return file.Some();
      }
      else if (folders.Next() is (true, var folder))
      {
         return folder.Some();
      }
      else
      {
         return nil;
      }
   }
}