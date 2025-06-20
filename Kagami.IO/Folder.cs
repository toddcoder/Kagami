﻿using Kagami.Library.Objects;
using Core.Computers;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.IO;

public class Folder : IObject, ICollection
{
   protected FolderName folderName;

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

   public Guid Id { get; init; } = Guid.NewGuid();

   public IIterator GetIterator(bool lazy) => new FolderIterator(this);

   public Maybe<IObject> Next(int index) => nil;

   public Maybe<IObject> Peek(int index) => nil;

   public Int Length => folderName.FileCount + folderName.Folders.Count();

   public bool ExpandForArray => true;

   public KBoolean In(IObject item)
   {
      return item switch
      {
         File f => folderName.Files.Contains(new FileName(f.AsString)),
         Folder folder => folderName.Folders.Contains(new FolderName(folder.AsString)),
         _ => false
      };
   }

   public KBoolean NotIn(IObject item)
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

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject Flatten() => this;

   public IObject this[SkipTake skipTake] => Library.Objects.CollectionFunctions.skipTake(this, skipTake);
}