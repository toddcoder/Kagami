using System.Collections.Generic;
using System.Linq;
using Core.Arrays;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
   public struct ByteArray : IObject, ICollection, IObjectCompare
   {
      byte[] bytes;

      public ByteArray(byte[] bytes) : this() => this.bytes = bytes;

      public string ClassName => "ByteArray";

      public string AsString => bytes.ToString(" ");

      public string Image => $"b\"{bytes.Select(b => (char)b).ToString("")}\"";

      public int Hash => bytes.GetHashCode();

      IEnumerable<IObject> list() => bytes.Select(Byte.ByteObject);

      public bool IsEqualTo(IObject obj) => obj is ByteArray ba && compareEnumerables(list(), ba.list());

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => bytes.Length > 0;

      public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

      public IMaybe<IObject> Next(int index)
      {
         var self = this;
         return maybe(index < bytes.Length, () => Byte.ByteObject(self.bytes[index]));
      }

      public IMaybe<IObject> Peek(int index) => Next(index);

      public Int Length => bytes.Length;

      public bool ExpandForArray => true;

      public Boolean In(IObject item) => item is Byte b && bytes.Contains(b.Value);

      public Boolean NotIn(IObject item) => !(item is Byte b && bytes.Contains(b.Value));

      public IObject Times(int count) => new ByteArray(bytes.Repeat(count));

      public String MakeString(string connector) => makeString(this, connector);

      public IIterator GetIndexedIterator() => new IndexedIterator(this);

      public int Compare(IObject obj) => compareCollections(this, obj);

      public IObject Object => this;

      public Boolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

      public Boolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

      public Byte this[int index] => bytes[wrapIndex(index, bytes.Length)];

      public IObject this[SkipTake skipTake] => skipTakeThis(this, skipTake);
   }
}