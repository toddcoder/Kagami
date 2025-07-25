using Core.Collections;
using Core.Computers;
using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.Objects.ObjectFunctions;
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

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

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