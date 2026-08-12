using Core.Collections;
using Core.Computers;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Packages;

public class Folder : IObject, ICollection
{
   protected FolderName folderName;

   public Folder(string folderName)
   {
      this.folderName = folderName;
   }

   public IIterator Files => new EnumerableIterator(folderName.Files.Select(f => new File(f.ToString())));

   public IIterator Folders => new EnumerableIterator(folderName.Folders.Select(f => new Folder(f.FullPath)));

   public string ClassName => "Folder";

   public string AsString => folderName.FullPath;

   public string Image => folderName.FullPath;

   public int Hash => folderName.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Folder f && folderName.FullPath == f.AsString;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings)
   {
      switch (comparisand)
      {
         case Any:
            return true;
         case Placeholder placeholder:
         {
            bindings[placeholder.Name] = this;
            return true;
         }
         case Folder otherFolder:
         {
            return folderName.FullPath == otherFolder.AsString;
         }
         default:
            return false;
      }
   }

   public bool IsTrue => false;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IIterator GetIterator(bool lazy) => new FolderIterator(this);

   public Maybe<IObject> Next(int index) => nil;

   public Maybe<IObject> Peek(int index) => nil;

   public Int Length => folderName.FileCount + folderName.Folders.Count();

   public bool ExpandForArray => true;

   public KBoolean In(IObject item) => item switch
   {
      File f => folderName.Files.Contains(new FileName(f.AsString)),
      Folder folder => folderName.Folders.Contains(new FolderName(folder.AsString)),
      _ => false
   };

   public KBoolean NotIn(IObject item) => item switch
   {
      File f => !folderName.Files.Contains(new FileName(f.AsString)),
      Folder folder => !folderName.Folders.Contains(new FolderName(folder.AsString)),
      _ => true
   };

   public IObject Times(int count) => this;

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject One() => this;

   public IObject Copy() => new Folder(folderName.FullPath);

   public IIterator Following(IObject following) => new MultiIterator(this, following);

   public Maybe<TypeConstraint> TypeConstraint => nil;

   public IObject Flatten() => this;

   public IObject this[SkipTake skipTake] => Library.Objects.CollectionFunctions.skipTake(this, skipTake);

   public File Combine(string fileExtension) => new(Path.Combine(folderName.FullPath, fileExtension));
}