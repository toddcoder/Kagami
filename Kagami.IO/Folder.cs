﻿using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Objects;
using Standard.Computer;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.IO
{
   public class Folder : IObject, ICollection
   {
      FolderName folderName;

      public Folder(string folderName) => this.folderName = folderName;

      public List<File> Files => folderName.Files.Select(f => new File(f.ToString())).ToList();

      public List<Folder> Folders => folderName.Folders.ToArray().Select(f => new Folder(f.FullPath)).ToList();

      public string ClassName => "Folder";

      public string AsString => folderName.FullPath;

      public string Image => folderName.FullPath;

      public int Hash => folderName.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Folder f && folderName.FullPath == f.AsString;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => false;

      public IIterator GetIterator(bool lazy) => new FolderIterator(this);

      public IMaybe<IObject> Next(int index) => none<IObject>();

      public IMaybe<IObject> Peek(int index) => none<IObject>();

      public Int Length => folderName.FileCount + folderName.Folders.Count();

      public bool ExpandForArray => true;

      public Boolean In(IObject item)
      {
         switch (item)
         {
            case File f:
               return folderName.Files.Contains(new FileName(f.AsString));
            case Folder folder:
               return folderName.Folders.Contains(new FolderName(folder.AsString));
            default:
               return false;
         }
      }

      public Boolean NotIn(IObject item)
      {
         switch (item)
         {
            case File f:
               return !folderName.Files.Contains(new FileName(f.AsString));
            case Folder folder:
               return !folderName.Folders.Contains(new FolderName(folder.AsString));
            default:
               return true;
         }
      }

      public IObject Times(int count) => this;

      public IObject Flatten() => this;
   }
}