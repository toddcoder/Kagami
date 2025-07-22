using Core.Collections;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public struct Words() : IObject, ICollection, IEqualityComparer<Word>
{
   private Word[] words = [];

   private static List<Word> fromString(KString kString)
   {
      List<Word> words = [];
      var _result = kString.Value.Matches("/(-/w*)/(/w+)/(-/w*)");
      if (_result is (true, var result))
      {
         foreach (var match in result)
         {
            words.Add(new Word(match.FirstGroup, match.SecondGroup, match.ThirdGroup));
         }
      }

      return words;
   }

   public Words(KString kString) : this()
   {
      words = [.. fromString(kString)];
   }

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);

   public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

   public Maybe<IObject> Next(int index) => index.Between(0).Until(words.Length) ? words[index] : nil;

   public Maybe<IObject> Peek(int index) => Next(index);

   public Int Length => words.Length;

   public bool ExpandForArray => false;

   public KBoolean In(IObject item) => (bool)words.FirstOrNone();

   public KBoolean NotIn(IObject item) => !In(item).Value;

   public IObject Times(int count) => this;

   public KString MakeString(string connector) => words.ToString(connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject One() => words.Length == 1 ? words[0] : new Words(words[0].Text);

   public bool Equals(Word x, Word y) => x.Text == y.Text;

   public int GetHashCode(Word obj) => obj.Text.GetHashCode();

   public string ClassName => "Words";

   public string AsString => words.Select(w => w.Text).ToString(" ");

   public string Image => words.Select(w => w.Image).ToString(", ");

   public int Hash => words.GetHashCode();

   public bool IsEqualTo(IObject obj) => isEqualTo(this, obj);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => words.Length > 0;

   public Guid Id { get; init; } = Guid.NewGuid();
}