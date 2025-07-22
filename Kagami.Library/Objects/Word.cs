using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Word(string prefix, string text, string suffix) : IObject, IEquatable<Word>
{
   public static Word Empty => new("", "", "");

   public string Prefix => prefix;

   public string Text => text;

   public string Suffix => suffix;

   public string ClassName => "Word";

   public string AsString => text;

   public string Image => $"<{prefix}|{text}|{suffix}>";

   public int Hash => HashCode.Combine(prefix, text, suffix);

   public bool IsEqualTo(IObject obj) => obj is Word otherWord && prefix == otherWord.Prefix && text == otherWord.Text && suffix == otherWord.Suffix;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => text.Length > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public bool Equals(Word other) => IsEqualTo(other);

   public override bool Equals(object? obj) => obj is Word other && Equals(other);

   public override int GetHashCode() => Hash;

   public static bool operator ==(Word left, Word right) => left.Equals(right);

   public static bool operator !=(Word left, Word right) => !left.Equals(right);
}