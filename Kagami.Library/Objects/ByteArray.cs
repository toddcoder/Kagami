using Core.Arrays;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public readonly struct ByteArray : IObject, ICollection, IObjectCompare
{
   private readonly byte[] bytes;

   public ByteArray(byte[] bytes) : this() => this.bytes = bytes;

   public string ClassName => "ByteArray";

   public string AsString => bytes.ToString(" ");

   public string Image => $"b\"{bytes.Select(b => (char)b).ToString("")}\"";

   public int Hash => bytes.GetHashCode();

   private IEnumerable<IObject> list() => bytes.Select(KByte.ByteObject);

   public bool IsEqualTo(IObject obj) => obj is ByteArray ba && compareEnumerables(list(), ba.list());

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => bytes.Length > 0;

   public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

   public Maybe<IObject> Next(int index) => index < bytes.Length ? KByte.ByteObject(bytes[index]).Some() : nil;

   public Maybe<IObject> Peek(int index) => Next(index);

   public Int Length => bytes.Length;

   public bool ExpandForArray => true;

   public KBoolean In(IObject item) => item is KByte b && bytes.Contains(b.Value);

   public KBoolean NotIn(IObject item) => !(item is KByte b && bytes.Contains(b.Value));

   public IObject Times(int count) => new ByteArray(bytes.Repeat(count));

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public int Compare(IObject obj) => compareCollections(this, obj);

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public KByte this[int index] => bytes[wrapIndex(index, bytes.Length)];

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);

   public ByteArray Concatenate(ByteArray other) => new([.. bytes.Concat(other.bytes)]);
}