using Core.Collections;
using Core.Computers;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Packages;

public class File : IObject, ICollection
{
   protected FileName fileName;

   public File(string fileName)
   {
      this.fileName = fileName;
   }

   public TextReader Reader() => new StreamReader(fileName.ReadingStream());

   public string ClassName => "File";

   public string AsString => fileName.ToString();

   public string Image => fileName.ToString();

   public int Hash => fileName.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is File f && fileName.ToString() == f.AsString;

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
         case KTuple { Length.Value: 3 } tuple:
         {
            if (tuple[0] is Placeholder p1)
            {
               bindings[p1.Name] = KString.StringObject(fileName.Folder.FullPath);
            }

            if (tuple[1] is Placeholder p2)
            {
               bindings[p2.Name] = KString.StringObject(fileName.Name);
            }

            if (tuple[2] is Placeholder p3)
            {
               bindings[p3.Name] = KString.StringObject(fileName.Extension);
            }

            return true;
         }
         case File otherFile:
         {
            return fileName.ToString() == otherFile.AsString;
         }
         default:
            return false;
      }
   }

   public bool IsTrue => fileName.Length > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IIterator GetIterator(bool lazy) => new FileIterator(this);

   public Maybe<IObject> Next(int index) => nil;

   public Maybe<IObject> Peek(int index) => nil;

   public Int Length => (int)fileName.Length;

   public bool ExpandForArray => true;

   public KBoolean In(IObject item) => fileName.Text.Contains(item.AsString);

   public KBoolean NotIn(IObject item) => !fileName.Text.Contains(item.AsString);

   public IObject Times(int count) => this;

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject One() => this;

   public IObject Copy() => new File(fileName.FullPath);

   public IIterator Following(IObject following) => new MultiIterator(this, following);

   public Maybe<TypeConstraint> TypeConstraint => nil;

   public IObject Flatten() => this;

   public KString Text => fileName.Text;

   public KArray Lines => new(fileName.Lines.Select(KString.StringObject).ToArray());

   public Folder Folder => new(fileName.Folder.FullPath);

   public KString Name => fileName.Name;

   public KString Extension => fileName.Extension;

   public KString NameExtension => fileName.NameExtension;

   public KString FullPath => fileName.FullPath;

   public IObject this[SkipTake skipTake] => Library.Objects.CollectionFunctions.skipTake(this, skipTake);
}