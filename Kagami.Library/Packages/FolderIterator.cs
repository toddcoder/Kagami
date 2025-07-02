using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Packages;

public class FolderIterator : Iterator
{
   protected Folder folder;
   protected List<File> files;
   protected List<Folder> folders;
   protected int fileIndex;
   protected int folderIndex;

   public FolderIterator(Folder folder) : base(folder)
   {
      this.folder = folder;
      files = [];
      folders = [];
      fileIndex = -1;
      folderIndex = -1;
   }

   public override Maybe<IObject> Next()
   {
      if (fileIndex == -1)
      {
         files = folder.Files;
         folders = folder.Folders;
         fileIndex = 0;
         folderIndex = 0;
      }

      if (fileIndex < files.Count)
      {
         var file = files[fileIndex++];
         return file;
      }
      else if (folderIndex < folders.Count)
      {
         var currentFolder = folders[folderIndex++];
         return currentFolder;
      }
      else
      {
         return nil;
      }
   }
}