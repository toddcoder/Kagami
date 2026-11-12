using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Word(string prefix, string text, string suffix, int index, int length) : IObject, IEquatable<Word>
{
   public static Word Empty => new("", "", "", -1, 0);

   public string Prefix => prefix;

   public string Text => text;

   public string Suffix => suffix;

   public int Index => index;

   public int Length => length;

   public string ClassName => "Word";

   public string AsString => text;

   public string Image => $"<{prefix}|{text}|{suffix}|{index}|{length}>";

   public int Hash => HashCode.Combine(prefix, text, suffix, index, length);

   public bool IsEqualTo(IObject obj) => obj is Word otherWord && prefix == otherWord.Prefix && text == otherWord.Text &&
      suffix == otherWord.Suffix && index == otherWord.Index && length == otherWord.Length;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => text.Length > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public bool Equals(Word other) => IsEqualTo(other);

   public override bool Equals(object? obj) => obj is Word other && Equals(other);

   public override int GetHashCode() => Hash;

   public static bool operator ==(Word left, Word right) => left.Equals(right);

   public static bool operator !=(Word left, Word right) => !left.Equals(right);
}