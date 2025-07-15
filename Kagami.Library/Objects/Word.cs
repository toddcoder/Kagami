using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Word(string word, int index) : IObject, IEquatable<Word>
{
   public static Word Empty => new("", -1);

   public string Text => word;

   public int Index => index;

   public string ClassName => "Word";

   public string AsString => word;

   public string Image => $"{word}@{index}";

   public int Hash => HashCode.Combine(word, index);

   public bool IsEqualTo(IObject obj) => obj is Word otherWord && word == otherWord.Text && index == otherWord.Index;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => word.Length > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public bool Equals(Word other) => IsEqualTo(other);

   public override bool Equals(object? obj) => obj is Word other && Equals(other);

   public override int GetHashCode() => Hash;

   public static bool operator ==(Word left, Word right) => left.Equals(right);

   public static bool operator !=(Word left, Word right) => !left.Equals(right);
}