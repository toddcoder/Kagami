﻿using System.Collections.Generic;
using Kagami.Library.Objects;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.IO
{
   public class FolderIterator : Iterator
   {
      Folder folder;
      List<File> files;
      List<Folder> folders;
      int fileIndex;
      int folderIndex;

      public FolderIterator(Folder folder) : base(folder)
      {
         this.folder = folder;
         files = new List<File>();
         folders = new List<Folder>();
         fileIndex = -1;
         folderIndex = -1;
      }

      public override IMaybe<IObject> Next()
      {
         if (fileIndex == -1)
         {
            files = folder.Files;
            folders = folder.Folders;
            fileIndex = 0;
            folderIndex = 0;
         }

         if (fileIndex<files.Count)
         {
            var file = files[fileIndex++];
            return file.Some<IObject>();
         }
         else if (folderIndex < folders.Count)
         {
            var currentFolder = folders[folderIndex++];
            return currentFolder.Some<IObject>();
         }
         else
            return none<IObject>();
      }
   }
}